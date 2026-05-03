import { useEffect, useRef } from 'react';
import Plotly from 'plotly.js-basic-dist';

interface PlotProps {
  data: any[];
  layout: Record<string, any>;
  config?: Record<string, any>;
  style?: React.CSSProperties;
  useResizeHandler?: boolean;
}

export function Plot({ data, layout, config, style, useResizeHandler }: PlotProps) {
  const elRef = useRef<HTMLDivElement>(null);
  const propsRef = useRef({ data, layout, config });
  const rafRef = useRef<number | null>(null);
  const lastDrawRef = useRef<number>(0);

  propsRef.current = { data, layout, config };

  useEffect(() => {
    const el = elRef.current;
    if (!el) return;

    const FRAME_INTERVAL = 200;

    const scheduleDraw = () => {
      if (rafRef.current !== null) return;
      rafRef.current = requestAnimationFrame(() => {
        rafRef.current = null;
        const now = performance.now();
        if (now - lastDrawRef.current >= FRAME_INTERVAL) {
          lastDrawRef.current = now;
          Plotly.react(el, propsRef.current.data, propsRef.current.layout, propsRef.current.config);
        } else {
          scheduleDraw();
        }
      });
    };

    scheduleDraw();

    return () => {
      if (rafRef.current !== null) {
        cancelAnimationFrame(rafRef.current);
        rafRef.current = null;
      }
    };
  }, [data, layout]);

  useEffect(() => {
    const el = elRef.current;
    if (!el) return;
    if (!useResizeHandler) return;
    const observer = new ResizeObserver(() => {
      Plotly.Plots.resize(el);
    });
    observer.observe(el);
    return () => {
      observer.disconnect();
      Plotly.purge(el);
    };
  }, [useResizeHandler]);

  return <div ref={elRef} style={style} />;
}