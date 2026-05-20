import * as React from 'react';

import { useTheme } from 'next-themes';

export const META_THEME_COLORS = {
    light: '#ffffff',
    dark: '#09090b'
};

export function useMetaColor() {
    const { resolvedTheme } = useTheme();

    const metaColor = React.useMemo(() => {
        return resolvedTheme !== 'dark' ? META_THEME_COLORS.light : META_THEME_COLORS.dark;
    }, [resolvedTheme]);

    const setMetaColor = React.useCallback((color: string) => {
        let meta = document.querySelector('meta[name="theme-color"]');
        if (!meta) {
            meta = document.createElement('meta');
            meta.setAttribute('name', 'theme-color');
            document.head.appendChild(meta);
        }
        meta.setAttribute('content', color);
    }, []);

    return {
        metaColor,
        setMetaColor
    };
}
