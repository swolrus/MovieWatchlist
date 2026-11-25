import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    })
      .compileComponents();
  });

  it('should send an empty movie request and print the response', () => {
    const logSpy = vi.spyOn(console, 'log').mockImplementation(() => {});
    const fixture = TestBed.createComponent(AppComponent);
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Movies');

    const http = TestBed.inject(HttpTestingController);
    const request = http.expectOne('/api/movies');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({});

    const response = { message: 'Validation failed' };
    request.flush(response, { status: 400, statusText: 'Bad Request' });
    expect(logSpy).toHaveBeenCalledWith('Response:', response);

    logSpy.mockRestore();
  });
});
