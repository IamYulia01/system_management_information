using System;
using System.Collections.Generic;

namespace system_management_information;

public partial class PhotoSight
{
    public int IdPhotoSight { get; set; }

    public string LinkPhoto { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public int IdSight { get; set; }

    public virtual Sight IdSightNavigation { get; set; } = null!;
}
