namespace Kendoku.Models;

public record Cell(int GroupIndex, int Row, int Col);

public record HelpCell(int GroupIndex, int Row, int Col, int Value) : Cell(GroupIndex, Row, Col);