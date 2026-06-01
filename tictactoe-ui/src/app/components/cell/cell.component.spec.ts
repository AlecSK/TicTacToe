import { TestBed, ComponentFixture } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { CellComponent } from './cell.component';

describe('CellComponent', () => {
  let fixture: ComponentFixture<CellComponent>;
  let component: CellComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CellComponent],
      providers: [provideZonelessChangeDetection()]
    }).compileComponents();

    fixture = TestBed.createComponent(CellComponent);
    component = fixture.componentInstance;
  });

  it('renders empty when mark is -', async () => {
    component.mark = '-';
    fixture.detectChanges();
    await fixture.whenStable();
    const btn: HTMLButtonElement = fixture.nativeElement.querySelector('button');
    expect(btn.textContent!.trim()).toBe('');
    expect(btn.classList.contains('x-mark')).toBe(false);
    expect(btn.classList.contains('o-mark')).toBe(false);
  });

  it('renders X with x-mark class', async () => {
    component.mark = 'X';
    fixture.detectChanges();
    await fixture.whenStable();
    const btn: HTMLButtonElement = fixture.nativeElement.querySelector('button');
    expect(btn.textContent!.trim()).toBe('X');
    expect(btn.classList.contains('x-mark')).toBe(true);
  });

  it('renders O with o-mark class', async () => {
    component.mark = 'O';
    fixture.detectChanges();
    await fixture.whenStable();
    const btn: HTMLButtonElement = fixture.nativeElement.querySelector('button');
    expect(btn.textContent!.trim()).toBe('O');
    expect(btn.classList.contains('o-mark')).toBe(true);
  });

  it('adds winning-cell class when isWinning is true', async () => {
    component.mark = 'X';
    component.isWinning = true;
    fixture.detectChanges();
    await fixture.whenStable();
    const btn: HTMLButtonElement = fixture.nativeElement.querySelector('button');
    expect(btn.classList.contains('winning-cell')).toBe(true);
  });

  it('emits cellClicked with index when empty cell clicked', async () => {
    component.mark = '-';
    component.disabled = false;
    component.index = 4;
    fixture.detectChanges();
    await fixture.whenStable();

    let emitted: number | undefined;
    component.cellClicked.subscribe((i: number) => emitted = i);

    component.onClick();
    expect(emitted).toBe(4);
  });

  it('does not emit when cell already has a mark', () => {
    component.mark = 'X';
    component.index = 0;

    let emitted = false;
    component.cellClicked.subscribe(() => emitted = true);

    component.onClick();
    expect(emitted).toBe(false);
  });

  it('does not emit when disabled is true', () => {
    component.mark = '-';
    component.disabled = true;
    component.index = 2;

    let emitted = false;
    component.cellClicked.subscribe(() => emitted = true);

    component.onClick();
    expect(emitted).toBe(false);
  });
});
