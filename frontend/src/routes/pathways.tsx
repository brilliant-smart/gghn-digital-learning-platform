import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { SiteShell } from "@/components/layout/SiteShell";
import { PageHeader } from "@/components/PageHeader";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { ArrowRight, GraduationCap } from "lucide-react";
import { pathwayApi, type PathwayDto } from "@/api/pathways";

export const Route = createFileRoute("/pathways")({
  head: () => ({
    meta: [
      { title: "Learning Pathways | GGHN Digital Learning" },
      { name: "description", content: "Curated learning journeys for public health professionals." },
      { property: "og:title", content: "Learning Pathways | GGHN Digital Learning" },
      { property: "og:description", content: "Curated learning journeys for public health professionals." },
    ],
  }),
  component: PathwaysPage,
});

function PathwaysPage() {
  const [pathways, setPathways] = useState<PathwayDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    pathwayApi.getAll()
      .then(setPathways)
      .catch(() => {})
      .finally(() => setLoading(false));
  }, []);

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
        eyebrow="Pathways"
        title="Structured learning journeys"
        description="Multi-resource pathways that build expertise in priority areas of public health."
      />
      <div className="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
        <div className="grid gap-6 md:grid-cols-2">
          {pathways.map((p) => (
            <Card key={p.id} className="flex h-full flex-col transition-all hover:shadow-[var(--shadow-elevated)]">
              <CardHeader>
                <div className="flex h-10 w-10 items-center justify-center rounded-md bg-primary-soft text-primary">
                  <GraduationCap className="h-5 w-5" />
                </div>
                <CardTitle className="mt-3 text-xl">{p.title}</CardTitle>
                <CardDescription>{p.description}</CardDescription>
              </CardHeader>
              <CardContent className="mt-auto flex items-center justify-between">
                <Badge variant="outline">{p.resourceCount} resources</Badge>
                <Button>Start Pathway <ArrowRight className="ml-1.5 h-4 w-4" /></Button>
              </CardContent>
            </Card>
          ))}
        </div>
      </div>
    </SiteShell>
  );
}