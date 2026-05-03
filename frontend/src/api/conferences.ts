import { api } from './client';

export interface SpeakerDto {
  id: string;
  name: string;
  title: string;
  bio?: string;
  organization: string;
  photoUrl?: string;
}

export interface SessionDto {
  id: string;
  title: string;
  description?: string;
  track: string;
  startTime: string;
  endTime: string;
  location?: string;
  virtualLink?: string;
  recordingUrl?: string;
  slideDeckUrl?: string;
  sessionSummary?: string;
  isPublished: boolean;
  speaker?: SpeakerDto;
}

export interface SponsorDto {
  id: string;
  name: string;
  tier: string;
  logoUrl?: string;
  websiteUrl?: string;
}

export interface ConferenceDto {
  id: string;
  title: string;
  theme: string;
  description: string;
  startDate: string;
  endDate: string;
  venue: string;
  registrationUrl?: string;
  year: number;
  isArchived: boolean;
  imageUrl?: string;
  sessions: SessionDto[];
  sponsors: SponsorDto[];
  createdAt: string;
}

export interface CreateConferenceRequest {
  title: string;
  theme: string;
  description: string;
  startDate: string;
  endDate: string;
  venue: string;
  registrationUrl?: string;
  year: number;
}

export interface UpdateConferenceRequest extends CreateConferenceRequest {
  isArchived: boolean;
  imageUrl?: string;
}

export interface CreateRegistrationRequest {
  conferenceId: string;
  firstName: string;
  lastName: string;
  email: string;
  organization: string;
  jobTitle: string;
  country: string;
  phoneNumber?: string;
  registrationType: string;
  dietaryRestrictions?: string;
  accessibilityNeeds?: string;
  specialRequests?: string;
}

export interface RegistrationDto {
  id: string;
  conferenceId: string;
  conferenceTitle?: string;
  userId?: string;
  firstName: string;
  lastName: string;
  email: string;
  organization: string;
  jobTitle: string;
  country: string;
  phoneNumber?: string;
  registrationType: string;
  status: string;
  dietaryRestrictions?: string;
  accessibilityNeeds?: string;
  specialRequests?: string;
  reviewedBy?: string;
  reviewedAt?: string;
  rejectionReason?: string;
  notes?: string;
  createdAt: string;
}

export interface UpdateRegistrationStatusRequest {
  status: string;
  rejectionReason?: string;
  notes?: string;
}

export interface RegistrationStatsDto {
  totalRegistrations: number;
  pending: number;
  approved: number;
  rejected: number;
  waitlisted: number;
}

export interface CreateSponsorRequest {
  name: string;
  tier: string;
  logoUrl?: string;
  websiteUrl?: string;
  conferenceId: string;
}

export interface UpdateSponsorRequest {
  name: string;
  tier: string;
  logoUrl?: string;
  websiteUrl?: string;
}

export const conferenceApi = {
  getAll: () => api.get<ConferenceDto[]>('/conferences'),
  getById: (id: string) => api.get<ConferenceDto>(`/conferences/${id}`),
  create: (data: CreateConferenceRequest) => api.post<ConferenceDto>('/conferences', data),
  update: (id: string, data: UpdateConferenceRequest) => api.put<ConferenceDto>(`/conferences/${id}`, data),
  delete: (id: string) => api.delete(`/conferences/${id}`),

  getSpeakers: () => api.get<SpeakerDto[]>('/speakers'),
  createSpeaker: (data: { name: string; title: string; organization: string; bio?: string }) =>
    api.post<SpeakerDto>('/speakers', data),
  updateSpeaker: (id: string, data: { name: string; title: string; organization: string; bio?: string; photoUrl?: string }) =>
    api.put<SpeakerDto>(`/speakers/${id}`, data),
  deleteSpeaker: (id: string) => api.delete(`/speakers/${id}`),

  register: (data: CreateRegistrationRequest) => api.post<RegistrationDto>('/registrations', data),
  getMyRegistrations: () => api.get<RegistrationDto[]>('/registrations/mine'),
  getRegistrationById: (id: string) => api.get<RegistrationDto>(`/registrations/${id}`),
  getRegistrationsByConference: (conferenceId: string, status?: string) =>
    api.get<RegistrationDto[]>(`/registrations/conference/${conferenceId}${status ? `?status=${status}` : ''}`),
  updateRegistrationStatus: (id: string, data: UpdateRegistrationStatusRequest) =>
    api.put<RegistrationDto>(`/registrations/${id}/status`, data),
  getRegistrationStats: (conferenceId: string) =>
    api.get<RegistrationStatsDto>(`/registrations/conference/${conferenceId}/stats`),

  createSponsor: (data: CreateSponsorRequest) => api.post<SponsorDto>('/sponsors', data),
  updateSponsor: (id: string, data: UpdateSponsorRequest) => api.put<SponsorDto>(`/sponsors/${id}`, data),
  deleteSponsor: (id: string) => api.delete(`/sponsors/${id}`),
};
