import { Component, Input, Output, EventEmitter, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CellComponent } from '../cell/cell.component';
import { CellMark } from '../../models/models';

const WIN_LINES = [
  [0, 1, 2], [3, 4, 5], [6, 7, 8],
  [0, 3, 6], [1, 4, 7], [2, 5, 8],
  [0, 4, 8], [2, 4, 6]
];

@Component({
  selector: 'app-board',
  standalone: true,
  imports: [CommonModule, CellComponent],
  templateUrl: './board.component.html',
  styleUrl: './board.component.scss'
})
export class BoardComponent implements OnChanges {
  @Input() boardState = '---------';
  @Input() disabled = false;
  @Input() playerMark: CellMark = 'X';

  @Output() cellClicked = new EventEmitter<number>();

  cells: CellMark[] = [];
  winningCells: Set<number> = new Set();

  ngOnChanges(): void {
    this.cells = this.boardState.split('') as CellMark[];
    this.winningCells = this.findWinningCells();
  }

  private findWinningCells(): Set<number> {
    for (const [a, b, c] of WIN_LINES) {
      const mark = this.cells[a];
      if (mark !== '-' && mark === this.cells[b] && mark === this.cells[c]) {
        return new Set([a, b, c]);
      }
    }
    return new Set();
  }
}
