import { useState, useEffect, lazy, Suspense } from "react";

const VenueMapInner = lazy(() =>
  import("./VenueMapInner").then((m) => ({ default: m.VenueMapInner }))
);

export function VenueMap() {
  const [mounted, setMounted] = useState(false);
  useEffect(() => setMounted(true), []);

  if (!mounted) {
    return (
      <div className="h-64 rounded-xl bg-gradient-to-br from-muted to-muted/50 flex items-center justify-center">
        <span className="text-muted-foreground font-medium">Loading map…</span>
      </div>
    );
  }

  return (
    <Suspense
      fallback={
        <div className="h-64 rounded-xl bg-gradient-to-br from-muted to-muted/50 flex items-center justify-center">
          <span className="text-muted-foreground font-medium">Loading map…</span>
        </div>
      }
    >
      <VenueMapInner />
    </Suspense>
  );
}