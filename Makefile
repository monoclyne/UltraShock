DEFAULT_UK_DIR := $(HOME)/.local/share/Steam/steamapps/common/ULTRAKILL
UK_DIR ?= $(DEFAULT_UK_DIR)
BEPINEX_PLUGIN_DIR := $(UK_DIR)/BepInEx/plugins

CONFIG ?= Debug

VERSION = $(shell jq -r ".version_number" < manifest.json)
BUILT_DLL = ./bin/$(CONFIG)/netstandard2.1/UltraShock.dll

.PHONY: build
build: check
	UK_DIR=$(UK_DIR) dotnet build -c $(CONFIG)

.PHONY: release
release: CONFIG := Release
release: build

.PHONY: package
package: CONFIG := Release
package: release
	rm -fr /tmp/UltraShock
	mkdir -p /tmp/UltraShock/plugins
	cp $(BUILT_DLL) /tmp/UltraShock/plugins/monoclyne.UltraShock.dll
	cp manifest.json /tmp/UltraShock
	cp README.md /tmp/UltraShock
	cp LICENSE /tmp/UltraShock
	cp icon.png /tmp/UltraShock
	cd /tmp/UltraShock && zip -r UltraShock-$(VERSION).zip *
	cp /tmp/UltraShock/*zip .

.PHONY: install
install: check build
	mkdir -p $(BEPINEX_PLUGIN_DIR)
	cp $(BUILT_DLL) $(BEPINEX_PLUGIN_DIR)

.PHONY: check
check:
	@if [ ! -d "$(UK_DIR)" ]; then \
		echo "Directory '$(UK_DIR)' does not exist."; \
		echo "Please set envvar UK_DIR to your ULTRAKILL install path."; \
		echo "Default is '$(DEFAULT_UK_DIR)'"; \
		exit 1; \
	fi
	@grep -q "PLUGIN_VERSION = \"$(VERSION)\"" PluginInfo.cs || (echo "Version in PluginInfo.cs does not match manifest.json!" && exit 1)
	@grep -q "<Version>$(VERSION)</Version>" UltraShock.csproj || (echo "Version in UltraShock.csproj does not match manifest.json!" && exit 1)

.PHONY: clean
clean:
	rm -fr ./bin
	rm -fr ./obj
