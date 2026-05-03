import { api } from './client';

export interface InitializePaymentRequest {
  templateId: string;
}

export interface InitializePaymentResponse {
  authorizationUrl: string;
  reference: string;
}

export interface VerifyPaymentResponse {
  status: string;
  reference: string;
  amount: number;
  templateId?: string;
  downloadUrl?: string;
}

export const paymentApi = {
  initialize: (templateId: string) =>
    api.post<InitializePaymentResponse>('/payments/initialize', { templateId }),

  verify: (reference: string) =>
    api.get<VerifyPaymentResponse>(`/payments/verify/${reference}`),
};