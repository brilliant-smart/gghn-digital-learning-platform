import { api } from './client';

export interface UserDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  membershipTier: string;
  organization?: string;
  jobTitle?: string;
  country?: string;
  roles: string[];
}

export interface AuthResponse {
  token: string;
  expiresAt: string;
  refreshToken: string;
  user: UserDto;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  organization?: string;
  jobTitle?: string;
  country?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  organization?: string;
  jobTitle?: string;
  country?: string;
}

export const authApi = {
  register: (data: RegisterRequest) =>
    api.post<AuthResponse>('/auth/register', data),

  login: (data: LoginRequest) =>
    api.post<AuthResponse>('/auth/login', data),

  refresh: (refreshToken: string) =>
    api.post<AuthResponse>('/auth/refresh', { refreshToken }),

  me: () =>
    api.get<UserDto>('/auth/me'),

  updateProfile: (data: UpdateProfileRequest) =>
    api.put<UserDto>('/auth/profile', data),
};