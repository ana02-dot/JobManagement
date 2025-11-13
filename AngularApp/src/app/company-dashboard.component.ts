import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PostedJobsComponent, Job } from './posted-jobs.component';
import { ApplicantsComponent, Applicant } from './applicants.component';

@Component({
  selector: 'app-company-dashboard',
  standalone: true,
  imports: [CommonModule, PostedJobsComponent, ApplicantsComponent],
  template: `
    <div class="min-h-screen bg-slate-50">
      <header class="bg-white border-b sticky top-0 z-40 py-4 px-4">
        <div class="flex justify-between items-center max-w-5xl mx-auto">
          <div>
            <h2 class="text-xl font-semibold">JobTracker</h2>
            <div class="text-slate-600">{{companyName}}</div>
          </div>
          <button class="btn btn-outline" (click)="logout.emit()">Logout</button>
        </div>
      </header>
      <main class="container mx-auto px-4 py-6 max-w-5xl">
        <div class="flex gap-4 mb-6">
          <button class="btn" [class.btn-primary]="currentTab==='jobs'" (click)="setTab('jobs')">Posted Jobs</button>
          <button class="btn" [class.btn-primary]="currentTab==='applicants'" (click)="setTab('applicants')">Applicants</button>
        </div>
        <ng-container *ngIf="currentTab==='jobs'">
          <app-posted-jobs [jobs]="postedJobs" (selectJob)="selectPostedJob($event)"></app-posted-jobs>
        </ng-container>
        <ng-container *ngIf="currentTab==='applicants'">
          <app-applicants [applicants]="applicants"></app-applicants>
        </ng-container>
      </main>
    </div>
  `
})
export class CompanyDashboardComponent {
  @Input() companyName = '';
  @Output() logout = new EventEmitter<void>();

  currentTab: 'jobs' | 'applicants' = 'jobs';

  postedJobs: Job[] = [
    {
      id: '1', position: 'Senior Frontend Developer', company: this.companyName, location: 'Tbilisi', employmentType: 'Full-time', category: 'Technology', salary: '₾4,000 - ₾6,000', postedDate: '2025-11-08', deadline: '2025-12-08', description: 'We are looking for an experienced Frontend Developer...', applicationsCount: 12,
    }
  ];
  applicants: Applicant[] = [
    { id: '1', name: 'John Doe', email: 'john.doe@example.com', appliedDate: '2025-11-09', cvName: 'john_doe_cv.pdf', status: 'new', jobId: '1', jobTitle: 'Senior Frontend Developer' },
    { id: '2', name: 'Jane Smith', email: 'jane.smith@example.com', appliedDate: '2025-11-08', cvName: 'jane_smith_cv.pdf', status: 'reviewing', jobId: '1', jobTitle: 'Senior Frontend Developer' },
  ];

  setTab(val: 'jobs' | 'applicants') { this.currentTab = val; }

  selectPostedJob(job: Job) {
    alert('Posted Job: ' + job.position);
  }
}
