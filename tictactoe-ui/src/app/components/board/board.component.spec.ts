import { TestBed, ComponentFixture } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { BoardComponent } from './board.component';

describe('BoardComponent', () => {
  let fixture: ComponentFixture<BoardComponent>;
  let component: BoardComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BoardComponent],
      providers: [provideZonelessChangeDetection()]
    }).compileComponents();

    fixture = TestBed.createComponent(BoardComponent);
    component = fixture.componentInstance;
  });

  it('renders 9 cells', async () => {
    component.boardState = '---------';
    component.ngOnChanges();
    fixture.detectChanges();
    await fixture.whenStable();
    const cells = fixture.nativeElement.querySelectorAll('app-cell');
    expect(cells.length).toBe(9);
  });

  it('parses boardState into individual cell marks', () => {
    component.boardState = 'XO-XO-XO-';
    component.ngOnChanges();
    expect(component.cells).toEqual(['X','O','-','X','O','-','X','O','-']);
  });

  it('detects horizontal winning line — top row', () => {
    component.boardState = 'XXX------';
    component.ngOnChanges();
    expect(component.winningCells.has(0)).toBe(true);
    expect(component.winningCells.has(1)).toBe(true);
    expect(component.winningCells.has(2)).toBe(true);
    expect(component.winningCells.has(3)).toBe(false);
  });

  it('detects horizontal winning line — bottom row', () => {
    component.boardState = '------OOO';
    component.ngOnChanges();
    expect(component.winningCells.has(6)).toBe(true);
    expect(component.winningCells.has(7)).toBe(true);
    expect(component.winningCells.has(8)).toBe(true);
  });

  it('detects vertical winning line — left column', () => {
    component.boardState = 'X--X--X--';
    component.ngOnChanges();
    expect(component.winningCells.has(0)).toBe(true);
    expect(component.winningCells.has(3)).toBe(true);
    expect(component.winningCells.has(6)).toBe(true);
    expect(component.winningCells.has(1)).toBe(false);
  });

  it('detects diagonal winning line — main diagonal', () => {
    component.boardState = 'X---X---X';
    component.ngOnChanges();
    expect(component.winningCells.has(0)).toBe(true);
    expect(component.winningCells.has(4)).toBe(true);
    expect(component.winningCells.has(8)).toBe(true);
  });

  it('detects diagonal winning line — anti-diagonal', () => {
    component.boardState = '--X-X-X--';
    component.ngOnChanges();
    expect(component.winningCells.has(2)).toBe(true);
    expect(component.winningCells.has(4)).toBe(true);
    expect(component.winningCells.has(6)).toBe(true);
  });

  it('no winning cells on empty board', () => {
    component.boardState = '---------';
    component.ngOnChanges();
    expect(component.winningCells.size).toBe(0);
  });

  it('no winning cells on mid-game board without winner', () => {
    component.boardState = 'XO-X-O---';
    component.ngOnChanges();
    expect(component.winningCells.size).toBe(0);
  });

  it('emits cellClicked when a cell emits', () => {
    component.boardState = '---------';
    component.ngOnChanges();

    let emitted: number | undefined;
    component.cellClicked.subscribe((i: number) => emitted = i);
    component.cellClicked.emit(4);

    expect(emitted).toBe(4);
  });
});
