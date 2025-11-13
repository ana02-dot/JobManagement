import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-auth-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="isOpen" class="fixed inset-0 flex items-center justify-center bg-black/30 z-50">
      <div class="bg-white rounded-lg shadow-lg w-full max-w-sm p-6 relative">
        <button aria-label="Close" class="absolute top-2 right-2 text-slate-400 hover:text-slate-900" (click)="close.emit()">✕</button>
        <div class="text-2xl font-bold mb-2">Welcome to Jobs.ge</div>
        <div class="flex mb-4 border rounded overflow-hidden">
          <button class="flex-1 py-2 text-center" [ngClass]="{'bg-slate-100': authType==='login'}" (click)="authType='login'">Login</button>
          <button class="flex-1 py-2 text-center" [ngClass]="{'bg-slate-100': authType==='signup'}" (click)="authType='signup'">Sign Up</button>
        </div>
        <form (ngSubmit)="handleSubmit()" class="space-y-4">
          <div *ngIf="authType==='signup'" class="space-y-1">
            <label for="signup-name" class="block">Full Name</label>
            <input id="signup-name" type="text" [(ngModel)]="formData.name" name="name" class="input input-bordered w-full" placeholder="John Doe" required/>
          </div>
          <div class="space-y-1">
            <label for="email" class="block">Email</label>
            <input id="email" type="email" [(ngModel)]="formData.email" name="email" class="input input-bordered w-full" placeholder="your.email@example.com" required/>
          </div>
          <div class="space-y-1">
            <label for="password" class="block">Password</label>
            <input id="password" type="password" [(ngModel)]="formData.password" name="password" class="input input-bordered w-full" placeholder="••••••••" required/>
          </div>
          <div>
            <label class="block mb-1">I am a:</label>
            <div class="flex gap-2">
              <label class="flex items-center gap-2 cursor-pointer flex-1 p-2 border rounded-lg hover:bg-slate-50">
                <input type="radio" name="role" [(ngModel)]="role" value="jobseeker" required />
                <span>
                  <span class="font-medium">Job Seeker</span><br/>
                  <span class="text-slate-500 text-sm">Looking for opportunities</span>
                </span>
              </label>
              <label class="flex items-center gap-2 cursor-pointer flex-1 p-2 border rounded-lg hover:bg-slate-50">
                <input type="radio" name="role" [(ngModel)]="role" value="company" required />
                <span>
                  <span class="font-medium">Company</span><br/>
                  <span class="text-slate-500 text-sm">Hiring talent</span>
                </span>
              </label>
            </div>
          </div>
          <button type="submit" class="btn btn-primary w-full">{{authType === 'login' ? 'Login' : 'Sign Up'}}</button>
        </form>
      </div>
    </div>
  `,
})
export class AuthModalComponent {
  @Input() isOpen: boolean = false;
  @Output() close = new EventEmitter<void>();
  @Output() auth = new EventEmitter<{ name: string; email: string; role: 'jobseeker' | 'company' }>();
  
  authType: 'login' | 'signup' = 'login';
  role: 'jobseeker' | 'company' = 'jobseeker';
  formData = { name: '', email: '', password: '' };

  handleSubmit() {
    this.auth.emit({
      name: this.formData.name || this.formData.email.split('@')[0],
      email: this.formData.email,
      role: this.role,
    });
    this.formData = { name: '', email: '', password: '' };
    this.close.emit();
  }
}
