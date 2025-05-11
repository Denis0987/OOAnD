using SpaceBattle.Lib.GameObjects;

namespace SpaceBattle.Tests.GameObjects;

public class GameObjectRepositoryTests
{
    [Fact]
    public void AddAndRetrieveObject_Success()
    {
        var repo = new GameObjectRepository();
        var obj = new GameObject();
        obj["hp"] = 100;

        repo.Add("id1", obj);
        var retrieved = repo.Get("id1");

        Assert.Equal(100, retrieved["hp"]);
    }

    [Fact]
    public void Add_Duplicate_Throws()
    {
        var repo = new GameObjectRepository();
        var obj = new GameObject();

        repo.Add("id1", obj);
        Assert.Throws<ArgumentException>(() => repo.Add("id1", new GameObject()));
    }

    [Fact]
    public void Get_NotExists_Throws()
    {
        var repo = new GameObjectRepository();
        Assert.Throws<KeyNotFoundException>(() => repo.Get("nope"));
    }

    [Fact]
    public void Remove_Success()
    {
        var repo = new GameObjectRepository();
        var obj = new GameObject();
        repo.Add("id", obj);

        repo.Remove("id");
        Assert.False(repo.Exists("id"));
    }

    [Fact]
    public void Remove_NotExists_Throws()
    {
        var repo = new GameObjectRepository();
        Assert.Throws<KeyNotFoundException>(() => repo.Remove("nope"));
    }
}
