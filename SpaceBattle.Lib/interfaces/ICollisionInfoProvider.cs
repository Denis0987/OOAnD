namespace SpaceBattle.Lib.Interfaces;

using System.Collections.Generic;

public interface ICollisionInfoProvider
{
    string FirstObjectId { get; }
    string SecondObjectId { get; }
    IList<int[]> GetCollisionPoints();
}
