import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="min-h-screen bg-gradient-to-b from-slate-50 to-white">
      <!-- Header -->
      <header class="bg-white border-b sticky top-0 z-50">
        <div class="container mx-auto px-4 max-w-7xl">
          <div class="flex items-center justify-between h-16">
            <div class="flex items-center gap-2">
              <div class="w-10 h-10 bg-blue-600 rounded-lg flex items-center justify-center">
                <span class="w-6 h-6 text-white">💼</span>
              </div>
              <h1 class="text-slate-900">Jobs.ge</h1>
            </div>
            <button class="btn btn-primary" (click)="getStarted.emit()">
              Get Started
            </button>
          </div>
        </div>
      </header>
      <!-- Hero Section -->
      <div class="relative overflow-hidden">
        <div class="absolute inset-0 bg-gradient-to-br from-blue-600/10 via-purple-600/10 to-pink-600/10"></div>
        <div class="container mx-auto px-4 py-16 max-w-7xl relative">
          <div class="text-center max-w-4xl mx-auto mb-12">
            <h1 class="text-5xl mb-6">
              Find Your Dream Job in Georgia
            </h1>
            <p class="text-slate-600 text-xl mb-8">
              Connect with top employers and track your applications in one place
            </p>
            <button class="btn btn-primary text-lg px-8 py-6" (click)="getStarted.emit()">
              Get Started
            </button>
          </div>
          <!-- Stats Section -->
          <div class="grid grid-cols-2 md:grid-cols-4 gap-6">
            <ng-container *ngFor="let stat of stats">
              <div class="text-center border-none shadow-lg rounded-lg bg-white py-6">
                <div class="w-8 h-8 mx-auto mb-3 text-blue-600">⭐</div>
                <div class="text-3xl mb-2">{{ stat.value }}</div>
                <div class="text-slate-600">{{ stat.label }}</div>
              </div>
            </ng-container>
          </div>
        </div>
      </div>
      <!-- Featured Jobs Section -->
      <div class="container mx-auto px-4 py-12 max-w-7xl">
        <div class="mb-8">
          <h2 class="text-3xl mb-2">Featured Jobs</h2>
          <p class="text-slate-600">Explore our top job opportunities</p>
        </div>
        <div class="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
          <ng-container *ngFor="let job of featuredJobs">
            <div (click)="getStarted.emit()" class="hover:shadow-lg transition-shadow cursor-pointer border rounded-lg bg-white">
              <div class="p-6 pb-2">
                <div class="text-xl mb-2">{{job.position}}</div>
                <div class="text-slate-600 mb-3">{{job.company}}</div>
                <div class="space-y-2">
                  <div class="flex items-center gap-2 text-slate-600">
                    <span class="w-4 h-4">📍</span>
                    <span>{{job.location}}</span>
                  </div>
                  <div class="flex items-center gap-2 text-slate-600">
                    <span class="w-4 h-4">💼</span>
                    <span>{{job.employmentType}}</span>
                  </div>
                  <div class="flex items-center gap-2 text-slate-600">
                    <span class="w-4 h-4">⏰</span>
                    <span>{{job.postedDate}}</span>
                  </div>
                </div>
              </div>
              <div class="p-6 pt-0">
                <span class="inline-block bg-slate-200 rounded px-3 py-1 text-sm">{{job.salary}}</span>
              </div>
            </div>
          </ng-container>
        </div>
      </div>
    </div>
  `,
})
export class HomeComponent {
  @Output() getStarted = new EventEmitter<void>();

  featuredJobs = [
    {
      id: '1',
      company: 'TechCorp Georgia',
      position: 'Senior Frontend Developer',
      location: 'Tbilisi',
      employmentType: 'Full-time',
      salary: '₾4,000 - ₾6,000',
      postedDate: '2 days ago',
    },
    {
      id: '2',
      company: 'Digital Solutions Ltd',
      position: 'UX/UI Designer',
      location: 'Tbilisi',
      employmentType: 'Full-time',
      salary: '₾3,000 - ₾4,500',
      postedDate: '3 days ago',
    },
    {
      id: '3',
      company: 'StartupHub',
      position: 'Marketing Manager',
      location: 'Remote',
      employmentType: 'Full-time',
      salary: '₾2,500 - ₾4,000',
      postedDate: '5 days ago',
    },
  ];

  stats = [
    { label: 'Active Jobs', value: '1,250+' },
    { label: 'Companies', value: '350+' },
    { label: 'Job Seekers', value: '15,000+' },
    { label: 'Success Rate', value: '87%' },
  ];
}
