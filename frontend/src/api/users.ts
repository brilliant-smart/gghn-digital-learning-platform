import { api } from './client';
import { type PagedResult } from './resources';
import { type UserDto } from './auth';

export interface UserListDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  membershipTier: string;
  organization?: string;
  country?: string;
  roles: string[];
  createdAt: string;
}

export const userApi = {
  getAll: (page?: number, pageSize?: number) => {
    const query = new URLSearchParams();
    if (page) query.set('page', page.toString());
    if (pageSize) query.set('pageSize', pageSize.toString());
    const qs = query.toString();
    return api.get<PagedResult<UserDto>>(`/auth/admin/users${qs ? `?${qs}` : ''}`);
  },

  getById: (userId: string) =>
    api.get<UserDto>(`/auth/admin/users/${userId}`),

  updateRole: (userId: string, role: string) =>
    api.put<{ message: string }>(`/auth/admin/users/${userId}/role?role=${encodeURIComponent(role)}`, {}),

  updateTier: (userId: string, tier: string) =>
    api.put<{ message: string }>(`/auth/admin/users/${userId}/tier?tier=${encodeURIComponent(tier)}`, {}),

  deleteUser: (userId: string) =>
    api.delete(`/auth/admin/users/${userId}`),
};