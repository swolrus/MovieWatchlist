import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';

interface Movie { id: number, watched: boolean, rating: number,comment: string, name: string}
interface Response {
  data: Movie[]
}
@Component({
  selector: 'app-root',
  standalone: true,
  template: `
    <main class="max-w-lg mx-auto mt-12 p-6 bg-white rounded-xl shadow-md">
      <h1 class="text-2xl font-bold text-slate-800 mb-4">Movies</h1>
      <ul class="space-y-2">
        @for (movie of movies().data; track movie.id) {
          <li class="p-3 bg-slate-50 rounded border flex justify-between">
            <span>{{ movie.name }}</span>
            <span class="text-sm font-medium text-emerald-600">
              {{ movie.watched ? 'Watched' : 'Pending' }}
            </span>
          </li>
        }
      </ul>
    </main>
  `
})
export class AppComponent {
  private http = inject(HttpClient);
  movies = signal<Response>({data: []});

  constructor() {
    this.http.get<Response>('/api/movies').subscribe((data) => {
      this.movies.set(data)
    });
  }
}