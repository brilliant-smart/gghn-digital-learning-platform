import { api } from './client';
import { type PagedResult } from './resources';

export interface PublicationDto {
  id: string;
  title: string;
  summary: string;
  author: string;
  content: string;
  status: string;
  imageUrl?: string;
  publicationType?: string;
  tags: string[];
  keyFindings: string[];
  externalUrl?: string;
  year?: number;
  publishedAt?: string;
  createdAt: string;
}

export interface PublicationFilterParams {
  search?: string;
  type?: string;
  tag?: string;
  year?: number;
  page?: number;
  pageSize?: number;
}

export const publicationApi = {
  getAll: (params?: PublicationFilterParams) => {
    const query = new URLSearchParams();
    if (params?.search) query.set('search', params.search);
    if (params?.type) query.set('type', params.type);
    if (params?.tag) query.set('tag', params.tag);
    if (params?.year) query.set('year', params.year.toString());
    if (params?.page) query.set('page', params.page.toString());
    if (params?.pageSize) query.set('pageSize', params.pageSize.toString());
    const qs = query.toString();
    return api.get<PagedResult<PublicationDto>>(`/publications${qs ? `?${qs}` : ''}`);
  },

  getById: (id: string) =>
    api.get<PublicationDto>(`/publications/${id}`),
};