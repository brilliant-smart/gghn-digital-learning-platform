import { api } from './client';

export interface LessonDto {
  id: string;
  title: string;
  durationMinutes: number;
  order: number;
  contentUrl?: string;
  description?: string;
  isPublished: boolean;
}

export interface CourseDto {
  id: string;
  title: string;
  description: string;
  topic: string;
  difficulty: string;
  durationMinutes: number;
  requiredTier: string;
  imageUrl?: string;
  lessons: LessonDto[];
  createdAt: string;
}

export const courseApi = {
  getAll: () => api.get<CourseDto[]>('/courses'),
  getById: (id: string) => api.get<CourseDto>(`/courses/${id}`),
};