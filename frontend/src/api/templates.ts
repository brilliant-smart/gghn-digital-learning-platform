import { api } from './client';

export interface TemplateDto {
  id: string;
  title: string;
  description: string;
  format: string;
  tier: string;
  price?: number;
  fileUrl?: string;
  createdAt: string;
}

export const templateApi = {
  getAll: () => api.get<TemplateDto[]>('/templates'),
  getById: (id: string) => api.get<TemplateDto>(`/templates/${id}`),
};