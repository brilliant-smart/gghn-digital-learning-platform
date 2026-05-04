const API_BASE_URL = import.meta.env.VITE_API_URL || '/api';

let isRefreshing = false;
let refreshPromise: Promise<string | null> | null = null;

export interface ProblemDetails {
  type: string;
  title: string;
  status: number;
  detail: string;
  errors?: string[];
  lockoutEnd?: string;
  email?: string;
}

export class ApiError extends Error {
  public readonly type: string;
  public readonly title: string;
  public readonly status: number;
  public readonly detail: string;
  public readonly errors?: string[];
  public readonly lockoutEnd?: string;
  public readonly email?: string;

  constructor(problem: ProblemDetails) {
    super(problem.detail);
    this.name = 'ApiError';
    this.type = problem.type;
    this.title = problem.title;
    this.status = problem.status;
    this.detail = problem.detail;
    this.errors = problem.errors;
    this.lockoutEnd = problem.lockoutEnd;
    this.email = problem.email;
  }
}

async function refreshToken(): Promise<string | null> {
  const refreshToken = typeof window !== 'undefined' ? localStorage.getItem('refresh_token') : null;
  if (!refreshToken) return null;

  try {
    const response = await fetch(`${API_BASE_URL}/auth/refresh`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken }),
    });

    if (!response.ok) {
      localStorage.removeItem('auth_token');
      localStorage.removeItem('refresh_token');
      return null;
    }

    const data = await response.json();
    localStorage.setItem('auth_token', data.token);
    localStorage.setItem('refresh_token', data.refreshToken);
    return data.token;
  } catch {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('refresh_token');
    return null;
  }
}

async function request<T>(endpoint: string, options?: RequestInit): Promise<T> {
  const token = typeof window !== 'undefined' ? localStorage.getItem('auth_token') : null;

  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...((options?.headers as Record<string, string>) || {}),
  };

  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  let response = await fetch(`${API_BASE_URL}${endpoint}`, {
    ...options,
    headers,
  });

  if (response.status === 401 && token) {
    if (!isRefreshing) {
      isRefreshing = true;
      refreshPromise = refreshToken().finally(() => {
        isRefreshing = false;
        refreshPromise = null;
      });
    }

    const newToken = refreshPromise ? await refreshPromise : await refreshToken();

    if (newToken) {
      headers['Authorization'] = `Bearer ${newToken}`;
      response = await fetch(`${API_BASE_URL}${endpoint}`, {
        ...options,
        headers,
      });
    } else {
      if (typeof window !== 'undefined') {
        window.location.href = '/auth';
      }
      throw new ApiError({
        type: 'https://tools.ietf.org/html/rfc7235#section-3.1',
        title: 'Session expired',
        status: 401,
        detail: 'Your session has expired. Please sign in again.',
      });
    }
  }

  if (!response.ok) {
    let problem: ProblemDetails;
    try {
      const body = await response.json();
      if (body.type && body.title && body.status !== undefined) {
        problem = body as ProblemDetails;
      } else {
        problem = {
          type: 'about:blank',
          title: response.statusText,
          status: body.StatusCode ?? body.status ?? response.status,
          detail: body.Message ?? body.message ?? body.detail ?? response.statusText,
        };
      }
    } catch {
      problem = {
        type: 'about:blank',
        title: response.statusText,
        status: response.status,
        detail: response.statusText,
      };
    }
    throw new ApiError(problem);
  }

  if (response.status === 204) return undefined as T;

  return response.json();
}

export const api = {
  get: <T>(endpoint: string) => request<T>(endpoint),

  post: <T>(endpoint: string, body: unknown) =>
    request<T>(endpoint, { method: 'POST', body: JSON.stringify(body) }),

  put: <T>(endpoint: string, body: unknown) =>
    request<T>(endpoint, { method: 'PUT', body: JSON.stringify(body) }),

  delete: <T>(endpoint: string) =>
    request<T>(endpoint, { method: 'DELETE' }),
};