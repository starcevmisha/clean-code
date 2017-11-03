namespace Chess
{
	public class ChessProblem
	{
		private static Board board;
		public static ChessStatus ChessStatus;

		public static void LoadFrom(string[] lines)
		{
			board = new BoardParser().ParseBoard(lines);
		}

		// Определяет мат, шах или пат белым.
		public static void CalculateChessStatus()
		{
			var isCheck = IsCheckForWhite();
			var hasMoves = false;
			foreach (var locFrom in board.GetPieces(PieceColor.White))
			{
				foreach (var locTo in board.GetPiece(locFrom).GetMoves(locFrom, board))
				{
					var old = board.GetPiece(locTo);
					board.Set(locTo, board.GetPiece(locFrom));
					board.Set(locFrom, null);
					if (!IsCheckForWhite())
						hasMoves = true;
					board.Set(locFrom, board.GetPiece(locTo));
					board.Set(locTo, old);
				}
			}
			if (isCheck)
				ChessStatus = hasMoves ? ChessStatus.Check : ChessStatus.Mate;
			else if (hasMoves) ChessStatus = ChessStatus.Ok;
			else ChessStatus = ChessStatus.Stalemate;
		}

		// check — это шах
		private static bool IsCheckForWhite()
		{
			foreach (var loc in board.GetPieces(PieceColor.Black))
			{
				var piece = board.GetPiece(loc);
				var moves = piece.GetMoves(loc, board);
				foreach (var destination in moves)
				{
				    if (board.GetPiece(destination).Is(PieceColor.White, PieceType.King))
				        return true;
				}
			}
		    return false;
		}
	}
}