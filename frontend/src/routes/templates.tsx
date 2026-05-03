import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { SiteShell } from "@/components/layout/SiteShell";
import { PageHeader } from "@/components/PageHeader";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Download, FileText, ShoppingCart, CheckCircle } from "lucide-react";
import { templateApi, type TemplateDto } from "@/api/templates";
import { paymentApi } from "@/api/payments";

export const Route = createFileRoute("/templates")({
  head: () => ({
    meta: [
      { title: "Resources | GGHN Digital Learning" },
      { name: "description", content: "Downloadable M&E frameworks, training tools, and field guides." },
      { property: "og:title", content: "Resources | GGHN Digital Learning" },
      { property: "og:description", content: "Downloadable M&E frameworks, training tools, and field guides." },
    ],
  }),
  component: TemplatesPage,
});

function TemplatesPage() {
  const [templates, setTemplates] = useState<TemplateDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [purchasing, setPurchasing] = useState<string | null>(null);
  const [purchased, setPurchased] = useState<Set<string>>(new Set());

  useEffect(() => {
    templateApi.getAll()
      .then(setTemplates)
      .catch(() => {})
      .finally(() => setLoading(false));

    // Check for payment reference in URL (redirect from Paystack)
    const params = new URLSearchParams(window.location.search);
    const reference = params.get('reference');
    if (reference) {
      paymentApi.verify(reference)
        .then((result) => {
          if (result.status === 'success' && result.templateId) {
            setPurchased(prev => new Set([...prev, result.templateId!]));
          }
          // Clean URL
          window.history.replaceState({}, '', '/templates');
        })
        .catch(() => {});
    }
  }, []);

  const handlePurchase = async (template: TemplateDto) => {
    if (!template.price) return;
    setPurchasing(template.id);
    try {
      const result = await paymentApi.initialize(template.id);
      window.location.href = result.authorizationUrl;
    } catch {
      setPurchasing(null);
    }
  };

  if (loading) {
    return (
      <SiteShell>
        <div className="flex items-center justify-center py-20">
          <p className="text-muted-foreground">Loading...</p>
        </div>
      </SiteShell>
    );
  }

  return (
    <SiteShell>
      <PageHeader
        eyebrow="Resources"
        title="Practical resources for public health work"
        description="Ready-to-use templates and tools to support program design, monitoring, and implementation."
      />
      <div className="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
        <div className="grid gap-5 md:grid-cols-2">
          {templates.map((t) => {
            const isPremium = t.tier === "Premium";
            const isPurchased = purchased.has(t.id);
            const isPurchasing = purchasing === t.id;

            return (
              <Card key={t.id}>
                <CardHeader>
                  <div className="flex items-start justify-between gap-3">
                    <div className="flex h-10 w-10 items-center justify-center rounded-md bg-primary-soft text-primary">
                      <FileText className="h-5 w-5" />
                    </div>
                    <div className="flex items-center gap-2">
                      <Badge variant="outline">{t.format}</Badge>
                      {isPremium && !isPurchased && (
                        <Badge variant="secondary" className="bg-secondary-soft text-secondary">Premium</Badge>
                      )}
                      {isPurchased && (
                        <Badge variant="secondary" className="bg-green-100 text-green-700">
                          <CheckCircle className="mr-1 h-3 w-3" /> Purchased
                        </Badge>
                      )}
                    </div>
                  </div>
                  <CardTitle className="mt-3 text-lg leading-snug">{t.title}</CardTitle>
                  <CardDescription>{t.description}</CardDescription>
                </CardHeader>
                <CardContent>
                  {isPremium && !isPurchased ? (
                    <div className="flex items-center gap-3">
                      <span className="text-lg font-semibold">₦{t.price?.toLocaleString()}</span>
                      <Button size="sm" onClick={() => handlePurchase(t)} disabled={isPurchasing}>
                        <ShoppingCart className="mr-2 h-4 w-4" />
                        {isPurchasing ? "Processing..." : "Buy Now"}
                      </Button>
                    </div>
                  ) : t.fileUrl || isPurchased ? (
                    <a href={t.fileUrl || '#'} target="_blank" rel="noreferrer">
                      <Button size="sm"><Download className="mr-2 h-4 w-4" /> Download</Button>
                    </a>
                  ) : (
                    <Button size="sm" variant="outline"><Download className="mr-2 h-4 w-4" /> Download</Button>
                  )}
                </CardContent>
              </Card>
            );
          })}
        </div>
      </div>
    </SiteShell>
  );
}