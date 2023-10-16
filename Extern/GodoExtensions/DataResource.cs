using Godot;
using Godot.Collections;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class DataResource : Resource
{
    [Export]
    public Dictionary<string, GodotObject> Data;
}
