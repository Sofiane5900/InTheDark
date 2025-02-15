using Godot;

public partial class Camera2d : Camera2D
{
    private TileMap _tileMap; // Je déclare une variable de type TileMap

    public override void _Ready()
    {
        // On récupere la TileMap
        _tileMap = GetNodeOrNull<TileMap>("../../TileMap");

        if (_tileMap == null)
        {
            GD.PrintErr("Erreur: TileMap introuvable");
            return;
        }
        Rect2I usedRect = _tileMap.GetUsedRect(); // Je retourne un rectangle qui englobe toutes les tiles de la TileMap
        Vector2 mapStart = _tileMap.MapToLocal(usedRect.Position); // Je récupère le début de mon rectangle
        Vector2 mapEnd = _tileMap.MapToLocal(usedRect.End); // Je récupère la fin de mon rectangle

        const int tileSize = 10; // Je déclare une constante qui correspond à la taille de mes tiles

        LimitLeft = (int)mapStart.X;
        LimitTop = (int)mapStart.Y;
        // Je leur soustrais une Tile pour éviter que la caméra affiche les bords de la map
        LimitRight = (int)(mapEnd.X - tileSize);
        LimitBottom = (int)(mapEnd.Y - tileSize);
    }
}
