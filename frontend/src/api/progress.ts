import { api } from './client';

export interface ProgressDto {
  id: string;
  courseId?: string;
  courseTitle?: string;
  lessonId?: string;
  lessonTitle?: string;
  pathwayId?: string;
  pathwayTitle?: string;
  isCompleted: boolean;
  completedAt?: string;
  certificateUrl?: string;
}

export const progressApi = {
  getMine: () => api.get<ProgressDto[]>('/progress/me'),
  markLessonComplete: (lessonId: string, courseId: string) =>
    api.post<ProgressDto>('/progress/lesson-complete', { lessonId, courseId }),
  markPathwayComplete: (pathwayId: string) =>
    api.post<ProgressDto>(`/progress/pathway-complete/${pathwayId}`, {}),
};