import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface Application {
  id: string;
  jobTitle: string;
  company: string;
  appliedDate: string;
  status: string;
  cvName: string;
  progress: number;
}

@Component({
  selector: 'app-my-applications',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div>
      <div *ngIf="!applications.length" class="text-slate-500 text-center p-8">No applications yet.</div>
      <div *ngFor="let app of applications" class="mb-4 border rounded-lg p-4">
        <div class="text-md font-semibold mb-1">{{app.jobTitle}}</div>
        <div class="flex gap-4 text-slate-500 text-sm">
          <span>{{app.company}}</span>
          <span>Applied: {{app.appliedDate}}</span>
          <span>Status: <span class="uppercase font-bold" [ngClass]="{
            'text-yellow-600': app.status==='reviewing',
            'text-blue-600': app.status==='interview',
            'text-green-600': app.status==='offer',
            'text-red-500': app.status==='rejected'
          }">{{app.status}}</span></span>
        </div>
        <div class="mt-2 text-xs">CV: {{app.cvName}} | Progress: {{app.progress}}%</div>
      </div>
    </div>
  `
})
export class MyApplicationsComponent {
  @Input() applications: Application[] = [];
}
