﻿import { DockviewComponent } from "./dockview-core.esm.js"
import { DockviewPanelContent } from "./dockview-content.js"
import { onAddGroup, addGroupWithPanel, toggleLock, disposeGroup } from "./dockview-group.js"
import { onAddPanel, onRemovePanel, getPanelsFromOptions, findContentFromPanels } from "./dockview-panel.js"
import { getConfig, reloadFromConfig, loadPanelsFromLocalstorage, saveConfig } from './dockview-config.js'
import './dockview-extensions.js'

const cerateDockview = (el, options) => {
    const template = el.querySelector('template');
    const dockview = new DockviewComponent({
        parentElement: el,
        createComponent: option => new DockviewPanelContent(option)
    });
    initDockview(dockview, options, template);

    dockview.init();
    return dockview;
}

const initDockview = (dockview, options, template) => {
    dockview.params = { panels: [], options, template, observer: null };
    loadPanelsFromLocalstorage(dockview);

    dockview.init = () => {
        const config = getConfig(options);
        dockview.params.floatingGroups = config.floatingGroups || []
        dockview.fromJSON(config);
    }

    dockview.update = options => {
        if (options.layoutConfig) {
            reloadFromConfig(dockview, options)
        }
        else if (dockview.params.options.lock !== options.lock) {
            dockview.params.options.lock = options.lock
            toggleGroupLock(dockview, options)
        }
        else {
            toggleComponent(dockview, options)
        }
    }

    dockview.reset = options => {
        reloadFromConfig(dockview, options)
    }

    dockview.onDidRemovePanel(onRemovePanel);

    dockview.onDidAddPanel(onAddPanel);

    dockview.onDidAddGroup(onAddGroup);

    dockview.onWillDragPanel(event => {
        if (event.panel.group.locked) {
            event.nativeEvent.preventDefault()
        }
    })

    dockview.onWillDragGroup(event => {
        if (event.group.locked) {
            event.nativeEvent.preventDefault()
        }
    })

    dockview.onDidLayoutFromJSON(() => {
        const handler = setTimeout(() => {
            clearTimeout(handler);

            const panels = dockview.panels
            const delPanelsStr = localStorage.getItem(dockview.params.options.localStorageKey + '-panels')
            const delPanels = delPanelsStr && JSON.parse(delPanelsStr) || []
            panels.forEach(panel => {
                dockview._panelVisibleChanged?.fire({ title: panel.title, status: true });
            })
            delPanels.forEach(panel => {
                dockview._panelVisibleChanged?.fire({ title: panel.title, status: false });
            })
            const { floatingGroups } = dockview.params
            floatingGroups.forEach(floatingGroup => {
                const group = dockview.groups.find(g => g.id == floatingGroup.data.id)
                if (!group) return
                const { width, height, top, left } = floatingGroup.position
                const style = group.element.parentElement.style
                style.width = width + 'px'
                style.height = height + 'px'
                style.top = top + 'px'
                style.left = left + 'px'
            })

            dockview._inited = true;
            dockview._initialized?.fire()
        }, 0);
    })
    // 拖拽分割线后触发
    dockview.gridview.onDidChange(event => {
        dockview._groupSizeChanged.fire()
        saveConfig(dockview)
    })

    dockview._rootDropTarget.onDrop(() => {
        saveConfig(dockview)
    })
}

const toggleComponent = (dockview, options) => {
    const panels = getPanelsFromOptions(options).filter(p => p.params.visible)
    const localPanels = dockview.panels
    panels.forEach(p => {
        const pan = findContentFromPanels(localPanels, p);
        if (pan === void 0) {
            const panel = findContentFromPanels(dockview.params.panels, p);
            const groupPanels = panels.filter(p1 => p1.params.parentId == p.params.parentId)
            const indexOfOptions = groupPanels.findIndex(p => p.id == panel.id)
            const index = panel && panel.params.index
            console.log(index, 'index');
            addGroupWithPanel(dockview, panel || p, panels, index ?? indexOfOptions);
        }
    })

    localPanels.forEach(item => {
        let pan = findContentFromPanels(panels, item);
        if (pan === void 0) {
            item.group.delPanelIndex = item.group.panels.findIndex(p => p.id == item.id)
            dockview.removePanel(item)
        }
    })
}
const toggleGroupLock = (dockview, options) => {
    dockview.groups.forEach(group => {
        toggleLock(group, group.header.rightActionsContainer, options.lock)
    })
}

export { cerateDockview };
