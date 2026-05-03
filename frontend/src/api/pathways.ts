import { api } from './client';

export interface PathwayDto {
  id: string;
  title: string;
  description: string;
  topic: string;
  learningObjective: string;
  estimatedDurationMinutes: number;
  imageUrl?: string;
  resourceCount: number;
  resources: { id: string; title: string; topic: string; audience: string; difficulty: string }[];
  createdAt: string;
}

export const pathwayApi = {
  getAll: () => api.get<PathwayDto[]>('/pathways'),
  getById: (id: string) => api.get<PathwayDto>(`/pathways/${id}`),
};