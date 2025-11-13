import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface Applicant {
  id: string;
  name: string;
  email: string;
  appliedDate: string;
  cvName: string;
  status: string;
  jobId: string;
  jobTitle: string;
}

@Component({
  selector: 'app-applicants',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div>
      <div *ngIf="!applicants.length" class="text-slate-500 text-center p-8">No applications yet.</div>
      <div *ngFor="let a of applicants" class="mb-4 border rounded-lg p-4 flex flex-col gap-1">
        <div class="font-semibold">{{a.name}} ({{a.email}})</div>
        <div class="flex gap-3 text-slate-600 text-sm">
          <span>Applied: {{a.appliedDate}}</span>
          <span>For: {{a.jobTitle}}</span>
          <span class="font-bold capitalize" [ngClass]="{
            'text-yellow-600': a.status==='reviewing',
            'text-blue-600': a.status==='interview',
            'text-green-600': a.status==='offer',
            'text-red-500': a.status==='rejected',
            'text-slate-900': a.status==='new'
          }">{{a.status}}</span>
        </div>
        <div class="text-xs">CV: {{a.cvName}}</div>
      </div>
    </div>
  `
})
export class ApplicantsComponent {
  @Input() applicants: Applicant[] = [];
}
