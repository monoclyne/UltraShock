# UltraShock

An ULTRAKILL mod to add support for OpenShock and PiShock shock collars.

Partially adapted from [PeakShock](https://github.com/addzeey/PeakShock/).

## Manual Install

This mod depends on BepInEx 5, so please install that first.
You can either use
[BepInExPack](https://thunderstore.io/c/ultrakill/p/BepInEx/BepInExPack/)
or directly get
[BepInEx](https://github.com/BepInEx/BepInEx).

Next, download and extract this mod's zip, and copy the `plugins`
directory into the `BepInEx` directory within the `ULTRAKILL` install.

After installation your ULTRAKILL directory should look, among other
files, like this:
```
/path/to/.local/share/Steam/steamapps/common/ULTRAKILL
├── BepInEx
│   ├── config
│   │   ├── BepInEx.cfg
│   ├── core
│   └── plugins
│       └── jakobbbb.UltraShock.dll
├── doorstop_config.ini
├── ULTRAKILL.exe
└── winhttp.dll
```

Note:  On **Linux** you may have to enter the following in ULTRAKILL's
[launch options](https://help.steampowered.com/en/faqs/view/7D01-D2DD-D75E-2955):
```
WINEDLLOVERRIDES="winhttp=n,b" %command%
```

## Configure

Before you can configure UltraShock, you'll have to start and then close
the game.

This should create a file called `jakobbbb.UltraShock.cfg` within the
`BepInEx/config` directory.  Open it in your favorite text editor.

First, find the line containing `Provider` and adapt it, depending on
whether you use OpenShock or PiShock.

You can also set a `ShockCooldownSeconds` value here, as well as
`ShockScale` from 0-100 here.  If the scale is, for example, 50, and you
receive 40 damage, you'll get a shock with an intensity of 20 (50% of
40).

Then, set the three options for your chosen provider.

For **OpenShock**, these are below the `[OpenShock]` section heading.

1. Create an API token [here](https://openshock.app/#/dashboard/tokens) and
paste it, without quotes, in the `ApiKey` line.
2. Go to the [list of shockers](https://openshock.app/#/dashboard/shockers/)
    and click "Edit" on the device you want to use.  Copy its ID into
    the `DeviceId` line.
3. Unless you know that you need to change it, you can keep `ApiUrl` as
   it is.

The final config will look something like this:

```
[OpenShock]
ApiKey = P0ywQl5Y2uOKHbgKrc7LpGvHgZvZtZ2W4UFZdxPMqZncXU8OWmswapCRupuFOFA0
DeviceId = bd4bae3e-155a-42cc-853f-9d404d670d3e
ApiUrl = https://api.openshock.app
```

For **PiShock**, set `APIKey`, `UserName` and `ShareCode` in a similar
way.

## Development (Linux)

First, you'll have to install the .NET SDK, e.g.
```
sudo pacman -S dotnet-sdk  # Arch
sudo snap install --classic dotnet-sdk  # Ubuntu
```

Then, you should be able to simply run `make` to build the mod and
`make install` to build and install it!

If you have installed ULTRAKILL in a non-default path, you'll have to
set the environment variable `UK_DIR`, e.g. like this:
```
UK_DIR=/mnt/storage/Steam/steamapps/common/ULTRAKILL make install
```

To build a `.zip` for distribution via Thunderstore, run `make package`.
