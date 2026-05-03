import { api } from './client';
import { type PagedResult } from './resources';

export interface DiscussionDto {
  id: string;
  content: string;
  parentId?: string;
  resourceId: string;
  userId: string;
  userName: string;
  createdAt: string;
  updatedAt?: string;
  replies: DiscussionDto[];
}

export interface CreateDiscussionRequest {
  resourceId: string;
  content: string;
}

export interface CreateReplyRequest {
  content: string;
}

export interface UpdateDiscussionRequest {
  content: string;
}

export const discussionApi = {
  getByResource: (resourceId: string, page?: number, pageSize?: number) => {
    const query = new URLSearchParams();
    if (page) query.set('page', page.toString());
    if (pageSize) query.set('pageSize', pageSize.toString());
    const qs = query.toString();
    return api.get<PagedResult<DiscussionDto>>(`/discussions/resource/${resourceId}${qs ? `?${qs}` : ''}`);
  },

  getById: (id: string) =>
    api.get<DiscussionDto>(`/discussions/${id}`),

  create: (data: CreateDiscussionRequest) =>
    api.post<DiscussionDto>('/discussions', data),

  reply: (id: string, data: CreateReplyRequest) =>
    api.post<DiscussionDto>(`/discussions/${id}/reply`, data),

  update: (id: string, data: UpdateDiscussionRequest) =>
    api.put<DiscussionDto>(`/discussions/${id}`, data),

  delete: (id: string) =>
    api.delete(`/discussions/${id}`),
};