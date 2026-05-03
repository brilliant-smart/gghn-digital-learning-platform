import { useState } from "react";
import { partners } from "@/data/partners";

export function PartnerCarousel() {
  const [failedImages, setFailedImages] = useState<Set<string>>(new Set());

  function handleImageError(src: string) {
    setFailedImages((prev) => new Set(prev).add(src));
  }

  return (
    <section className="overflow-hidden">
      <div
        className="pointer-events-none relative"
        style={{
          maskImage: "linear-gradient(to right, transparent, black 8%, black 92%, transparent)",
          WebkitMaskImage: "linear-gradient(to right, transparent, black 8%, black 92%, transparent)",
        }}
      >
        <div className="flex w-max gap-12 md:gap-16 animate-scroll hover:[animation-play-state:paused]">
          {partners.map((p) => (
            <LogoItem key={p.src} partner={p} failed={failedImages.has(p.src)} onError={() => handleImageError(p.src)} />
          ))}
          {partners.map((p) => (
            <LogoItem key={`dup-${p.src}`} partner={p} failed={failedImages.has(p.src)} onError={() => handleImageError(p.src)} />
          ))}
        </div>
      </div>
    </section>
  );
}

function LogoItem({
  partner,
  failed,
  onError,
}: {
  partner: { name: string; src: string };
  failed: boolean;
  onError: () => void;
}) {
  if (failed) {
    return (
      <div className="flex h-16 w-32 shrink-0 items-center justify-center rounded border border-muted bg-muted/30 text-sm text-muted-foreground">
        {partner.name}
      </div>
    );
  }

  return (
    <img
      src={partner.src}
      alt={partner.name}
      onError={onError}
      className="h-16 w-auto max-w-[160px] shrink-0 object-contain transition-all duration-300 hover:drop-shadow-md md:h-[64px]"
    />
  );
}