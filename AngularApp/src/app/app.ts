import { Component, Signal, WritableSignal, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeComponent } from './home.component';
import { AuthModalComponent } from './auth-modal.component';
import { JobSeekerDashboardComponent } from './jobseeker-dashboard.component';
import { CompanyDashboardComponent } from './company-dashboard.component';

// User and application interfaces
export interface JobListing {
  id: string;
  company: string;
  companyLogo: string;
  position: string;
  location: string;
  employmentType: 'Full-time' | 'Part-time' | 'Contract' | 'Internship';
  category: string;
  salary?: string;
  postedDate: string;
  deadline: string;
  description: string;
  requirements: string[];
  responsibilities: string[];
  benefits?: string[];
}

export interface Application {
  id: string;
  jobId: string;
  jobTitle: string;
  company: string;
  appliedDate: string;
  status: 'submitted' | 'reviewing' | 'interview' | 'offer' | 'rejected';
  cvName: string;
  progress: number;
}

interface User {
  name: string;
  email: string;
  role: 'jobseeker' | 'company';
}

@Component({
  selector: 'app-root',
  standalone: true,
  template: `
    <ng-container *ngIf="!user(); else dashboard">
      <app-home (getStarted)="showAuth = true"></app-home>
      <app-auth-modal
        [isOpen]="showAuth"
        (close)="showAuth = false"
        (auth)="handleAuth($event)"></app-auth-modal>
    </ng-container>
    <ng-template #dashboard>
      <ng-container *ngIf="user()?.role === 'company'; else seeker">
        <app-company-dashboard
          [companyName]="user()?.name || ''"
          (logout)="handleLogout()"
        ></app-company-dashboard>
      </ng-container>
      <ng-template #seeker>
        <app-jobseeker-dashboard
          [userName]="user()?.name || ''"
          (logout)="handleLogout()"
        ></app-jobseeker-dashboard>
      </ng-template>
    </ng-template>
  `,
  imports: [CommonModule, HomeComponent, AuthModalComponent, JobSeekerDashboardComponent, CompanyDashboardComponent]
})
export class AppComponent {
  user: WritableSignal<User | null> = signal<User | null>(null);
  showAuth = false;

  handleAuth(userData: User) {
    this.user.set(userData);
    this.showAuth = false;
  }

  handleLogout() {
    this.user.set(null);
  }
}
