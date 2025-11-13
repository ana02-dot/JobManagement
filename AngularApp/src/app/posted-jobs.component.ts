import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface Job {
  id: string;
  position: string;
  company: string;
  location: string;
  employmentType: string;
  category: string;
  salary: string;
  postedDate: string;
  deadline: string;
  description: string;
  applicationsCount: number;
}

@Component({
  selector: 'app-posted-jobs',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div>
      <div *ngIf="!jobs.length" class="text-slate-500 text-center p-8">No jobs posted yet.</div>
      <div *ngFor="let job of jobs" (click)="selectJob.emit(job)" class="mb-4 border rounded-lg p-4 cursor-pointer hover:shadow flex flex-col gap-2">
        <div class="text-lg font-semibold">{{job.position}}</div>
        <div class="flex gap-2 text-slate-500 text-sm flex-wrap">
          <span>{{job.location}}</span>
          <span>{{job.employmentType}}</span>
          <span>{{job.category}}</span>
          <span *ngIf="job.salary">Salary: {{job.salary}}</span>
        </div>
        <div class="text-xs text-right text-slate-400">Posted: {{job.postedDate}} | Deadline: {{job.deadline}}</div>
        <div class="mt-2 text-sm text-slate-700 line-clamp-2">{{job.description}}</div>
        <div class="mt-1 text-xs text-blue-600">Applicants: {{job.applicationsCount}}</div>
      </div>
    </div>
  `
})
export class PostedJobsComponent {
  @Input() jobs: Job[] = [];
  @Output() selectJob = new EventEmitter<Job>();
}
