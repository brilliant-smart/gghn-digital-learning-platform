import { api } from './client';

export interface ResourceDto {
  id: string;
  title: string;
  summary: string;
  plainLanguageSummary: string;
  sourceUrl: string;
  topic: string;
  audience: string;
  difficulty: string;
  status: string;
  geography?: string;
  format?: string;
  publicationDate?: string;
  takeaways: string[];
  createdAt: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface ResourceFilterParams {
  topic?: string;
  audience?: string;
  difficulty?: string;
  search?: string;
  page?: number;
  pageSize?: number;
}

export const resourceApi = {
  getAll: (params?: ResourceFilterParams) => {
    const query = new URLSearchParams();
    if (params?.topic) query.set('topic', params.topic);
    if (params?.audience) query.set('audience', params.audience);
    if (params?.difficulty) query.set('difficulty', params.difficulty);
    if (params?.search) query.set('search', params.search);
    if (params?.page) query.set('page', params.page.toString());
    if (params?.pageSize) query.set('pageSize', params.pageSize.toString());
    const qs = query.toString();
    return api.get<PagedResult<ResourceDto>>(`/resources${qs ? `?${qs}` : ''}`);
  },

  getById: (id: string) =>
    api.get<ResourceDto>(`/resources/${id}`),
};