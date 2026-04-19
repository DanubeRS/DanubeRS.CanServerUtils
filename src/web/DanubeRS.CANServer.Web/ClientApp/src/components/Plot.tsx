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

  propsRef.current = { data, layout, config };

  useEffect(() => {
    const el = elRef.current;
    if (!el) return;
    Plotly.react(el, propsRef.current.data, propsRef.current.layout, propsRef.current.config);
  });

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