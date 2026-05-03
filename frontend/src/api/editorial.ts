import { api } from './client';

export interface ResourceInReviewDto {
  id: string;
  title: string;
  summary: string;
  status: string;
  contributorId?: string;
  createdAt: string;
}

export interface EditorialReviewDto {
  id: string;
  status: string;
  reviewNotes?: string;
  resourceId: string;
  reviewerId?: string;
  reviewerName?: string;
  reviewedAt?: string;
  createdAt: string;
}

export interface CreateReviewRequest {
  resourceId: string;
  reviewNotes: string;
}

export interface UpdateReviewRequest {
  reviewNotes: string;
}

export const editorialApi = {
  getQueue: () =>
    api.get<ResourceInReviewDto[]>('/editorial/queue'),

  submitForReview: (resourceId: string) =>
    api.post<EditorialReviewDto>(`/editorial/resources/${resourceId}/submit`, {}),

  createReview: (data: CreateReviewRequest) =>
    api.post<EditorialReviewDto>('/editorial/reviews', data),

  updateReview: (id: string, data: UpdateReviewRequest) =>
    api.put<EditorialReviewDto>(`/editorial/reviews/${id}`, data),

  approve: (resourceId: string) =>
    api.post<{ message: string }>(`/editorial/resources/${resourceId}/approve`, {}),

  reject: (resourceId: string, reason?: string) =>
    api.post<{ message: string }>(`/editorial/resources/${resourceId}/reject?reason=${encodeURIComponent(reason || '')}`, {}),
};