import { api } from './client';

export interface DashboardStatsDto {
  totalUsers: number;
  totalResources: number;
  totalCourses: number;
  totalPathways: number;
  totalCompletions: number;
  totalDiscussions: number;
  totalPublications: number;
  totalConferences: number;
}

export interface TopResourceDto {
  id: string;
  title: string;
  topic: string;
  viewCount: number;
}

export interface TopPathwayDto {
  id: string;
  title: string;
  completionCount: number;
}

export interface GeographyStatDto {
  country: string;
  userCount: number;
}

export interface AudienceStatDto {
  membershipTier: string;
  userCount: number;
}

export const analyticsApi = {
  getDashboard: () =>
    api.get<DashboardStatsDto>('/analytics/dashboard'),

  getTopResources: (count?: number) =>
    api.get<TopResourceDto[]>(`/analytics/top-resources${count ? `?count=${count}` : ''}`),

  getTopPathways: (count?: number) =>
    api.get<TopPathwayDto[]>(`/analytics/top-pathways${count ? `?count=${count}` : ''}`),

  getByGeography: () =>
    api.get<GeographyStatDto[]>('/analytics/by-geography'),

  getByAudience: () =>
    api.get<AudienceStatDto[]>('/analytics/by-audience'),
};