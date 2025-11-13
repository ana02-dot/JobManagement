import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface Job {
  id: string;
  position: string;
  company: string;
  location: string;
  employmentType: string;
  salary: string;
  postedDate: string;
}

@Component({
  selector: 'app-job-listings',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div>
      <div *ngFor="let job of jobs" (click)="selectJob.emit(job)" class="p-4 mb-4 cursor-pointer border rounded hover:shadow flex flex-col gap-1">
        <div class="text-lg font-semibold">{{job.position}}</div>
        <div class="text-slate-600">{{job.company}}</div>
        <div class="flex gap-4 text-slate-500 text-sm">
          <span>{{job.location}}</span>
          <span>{{job.employmentType}}</span>
          <span>{{job.salary}}</span>
        </div>
        <div class="text-xs text-right text-slate-400">Posted: {{job.postedDate}}</div>
      </div>
    </div>
  `
})
export class JobListingsComponent {
  @Input() jobs: Job[] = [];
  @Output() selectJob = new EventEmitter<Job>();
}
