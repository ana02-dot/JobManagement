import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { JobListingsComponent, Job } from './job-listings.component';
import { MyApplicationsComponent, Application } from './my-applications.component';
import { ApiService } from './api.service';

@Component({
  selector: 'app-jobseeker-dashboard',
  standalone: true,
  imports: [CommonModule, JobListingsComponent, MyApplicationsComponent],
  template: `
    <div class="min-h-screen bg-slate-50">
      <header class="bg-white border-b sticky top-0 z-40 py-4 px-4">
        <div class="flex justify-between items-center max-w-5xl mx-auto">
          <h2 class="text-xl font-semibold">Welcome, {{userName}}</h2>
          <button class="btn btn-outline" (click)="logout.emit()">Logout</button>
        </div>
      </header>
      <main class="container mx-auto px-4 py-6 max-w-5xl">
        <div class="flex gap-4 mb-6">
          <button class="btn" [class.btn-primary]="currentView==='listings'" (click)="setView('listings')">Job Listings</button>
          <button class="btn" [class.btn-primary]="currentView==='applications'" (click)="setView('applications')">My Applications</button>
        </div>

        <ng-container *ngIf="currentView==='listings'">
          <div *ngIf="isLoading" class="text-slate-500">Loading jobs...</div>
          <div *ngIf="error" class="text-red-600">{{error}}</div>
          <app-job-listings *ngIf="!isLoading && !error" [jobs]="jobs" (selectJob)="showJobDetail($event)"></app-job-listings>
        </ng-container>

        <ng-container *ngIf="currentView==='applications'">
          <app-my-applications [applications]="applications"></app-my-applications>
        </ng-container>
      </main>
    </div>
  `
})
export class JobSeekerDashboardComponent implements OnInit {
  @Input() userName = '';
  @Output() logout = new EventEmitter<void>();

  currentView: 'listings' | 'applications' = 'listings';

  jobs: Job[] = [];
  isLoading = false;
  error = '';

  applications: Application[] = [
    {
      id: '1', jobTitle: 'Senior Frontend Developer', company: 'TechCorp Georgia', appliedDate: '2025-11-08', status: 'interview', cvName: 'john_doe_cv.pdf', progress: 60,
    },
  ];

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.fetchJobs();
  }

  private fetchJobs() {
    this.isLoading = true;
    this.error = '';
    this.api.getJobs().subscribe({
      next: (data: any) => {
        // Expecting backend to return array of jobs; map minimal fields for UI
        this.jobs = (data || []).map((j: any) => ({
          id: String(j.id ?? j.jobId ?? ''),
          position: j.position ?? j.title ?? 'Job',
          company: j.company ?? j.companyName ?? 'Company',
          location: j.location ?? 'Unknown',
          employmentType: j.employmentType ?? 'Full-time',
          salary: j.salary ?? '',
          postedDate: j.postedDate ?? j.createdAt ?? ''
        }));
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'Failed to load jobs. Please check API and CORS.';
        this.isLoading = false;
        // Optional: console.error(err);
      }
    });
  }

  setView(view: 'listings' | 'applications') {
    this.currentView = view;
  }

  showJobDetail(job: Job) {
    alert('Job: ' + job.position);
  }
}
