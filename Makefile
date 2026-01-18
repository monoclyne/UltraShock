DEFAULT_UK_DIR := $(HOME)/.local/share/Steam/steamapps/common/ULTRAKILL
UK_DIR ?= $(DEFAULT_UK_DIR)

CONFIG ?= Debug

BEPINEX_PLUGIN_DIR=$(UK_DIR)/BepInEx/plugins

.PHONY: build
build: check
	UK_DIR=$(UK_DIR) dotnet build -c $(CONFIG)

.PHONY: release
release: CONFIG := Release
release: build

.PHONY: install
install: check build
	cp ./bin/$(CONFIG)/netstandard2.1/UltraShock.dll $(BEPINEX_PLUGIN_DIR)

.PHONY: check
check:
	@if [ ! -d "$(UK_DIR)" ]; then \
		echo "Directory '$(UK_DIR)' does not exist."; \
		echo "Please set envvar UK_DIR to your ULTRAKILL install path."; \
		echo "Default is '$(DEFAULT_UK_DIR)'"; \
		exit 1; \
	fi

.PHONY: clean
clean:
	rm -fr ./bin
	rm -fr ./obj
