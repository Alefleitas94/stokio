import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface LoginRequest {
  subdomain: string;
  email: string;
  password: string;
}

export interface AuthenticationResult {
  success: boolean;
  token?: string;
  message?: string;
  tenantId?: number;
  userId?: number;
  email?: string;
  roles?: string[];
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);

  login(request: LoginRequest): Observable<AuthenticationResult> {
    return this.http.post<AuthenticationResult>(`${environment.apiUrl}/auth/login`, request);
  }

  setToken(token: string): void {
    localStorage.setItem('auth_token', token);
  }

  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  clearToken(): void {
    localStorage.removeItem('auth_token');
  }
}
