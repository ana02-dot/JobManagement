import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

// Adjust the base URL to your backend origin (from Rider run output)
const API_BASE_URL = 'http://localhost:5000'; // or https://localhost:5001 for HTTPS

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);
  private baseUrl = API_BASE_URL;

  // Jobs
  getJobs() {
    return this.http.get(`${this.baseUrl}/api/jobs`);
  }

  getJobById(id: string | number) {
    return this.http.get(`${this.baseUrl}/api/jobs/${id}`);
  }

  // Auth (adjust according to your API shape)
  login(payload: { email: string; password: string }) {
    return this.http.post(`${this.baseUrl}/api/auth/login`, payload);
  }

  register(payload: { name: string; email: string; password: string; role: string }) {
    return this.http.post(`${this.baseUrl}/api/auth/register`, payload);
  }
}
