import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { SiteShell } from "@/components/layout/SiteShell";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { BarChart3, Users, BookOpen, GraduationCap, Route as RouteIcon, CheckCircle2, MessageSquare, FileText, Calendar } from "lucide-react";
import { analyticsApi, type DashboardStatsDto, type TopResourceDto, type TopPathwayDto, type AudienceStatDto } from "@/api/analytics";
import { useAuth } from "@/lib/auth";
import { useNavigate } from "@tanstack/react-router";

export const Route = createFileRoute("/admin/analytics")({
  head: () => ({ meta: [{ title: "Analytics | GGHN Admin" }] }),
  component: AnalyticsPage,
});

function AnalyticsPage() {
  const { user, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const [stats, setStats] = useState<DashboardStatsDto | null>(null);
  const [topResources, setTopResources] = useState<TopResourceDto[]>([]);
  const [topPathways, setTopPathways] = useState<TopPathwayDto[]>([]);
  const [audience, setAudience] = useState<AudienceStatDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!isAuthenticated || !user?.roles?.some(r => r === "Admin" || r === "Editor")) {
      navigate({ to: "/dashboard", replace: true });
      return;
    }
    Promise.all([
      analyticsApi.getDashboard(),
      analyticsApi.getTopResources(5),
      analyticsApi.getTopPathways(5),
      analyticsApi.getByAudience(),
    ]).then(([s, r, p, a]) => {
      setStats(s);
      setTopResources(r);
      setTopPathways(p);
      setAudience(a);
    }).catch(() => {}).finally(() => setLoading(false));
  }, [isAuthenticated]);

  if (loading) return <SiteShell><div className="flex justify-center py-20"><p className="text-muted-foreground">Loading...</p></div></SiteShell>;

  const statCards = stats ? [
    { icon: Users, label: "Total Users", value: stats.totalUsers },
    { icon: BookOpen, label: "Resources", value: stats.totalResources },
    { icon: GraduationCap, label: "Courses", value: stats.totalCourses },
    { icon: RouteIcon, label: "Pathways", value: stats.totalPathways },
    { icon: CheckCircle2, label: "Completions", value: stats.totalCompletions },
    { icon: MessageSquare, label: "Discussions", value: stats.totalDiscussions },
    { icon: FileText, label: "Publications", value: stats.totalPublications },
    { icon: Calendar, label: "Conferences", value: stats.totalConferences },
  ] : [];

  return (
    <SiteShell>
      <div className="mx-auto max-w-6xl px-4 py-10 sm:px-6 lg:px-8">
        <div className="flex items-center gap-3 mb-8">
          <BarChart3 className="h-6 w-6 text-primary" />
          <h1 className="text-2xl font-bold tracking-tight">Analytics Dashboard</h1>
        </div>

        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
          {statCards.map((s) => (
            <Card key={s.label}>
              <CardContent className="flex items-center gap-4 p-5">
                <div className="flex h-11 w-11 items-center justify-center rounded-md bg-primary-soft text-primary">
                  <s.icon className="h-5 w-5" />
                </div>
                <div>
                  <p className="text-2xl font-bold tracking-tight">{s.value}</p>
                  <p className="text-xs text-muted-foreground">{s.label}</p>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>

        <div className="mt-10 grid gap-6 lg:grid-cols-2">
          <Card>
            <CardHeader><CardTitle className="text-base">Top Resources</CardTitle></CardHeader>
            <CardContent>
              {topResources.length === 0 ? <p className="text-sm text-muted-foreground">No data yet.</p> : (
                <ul className="space-y-3">
                  {topResources.map((r, i) => (
                    <li key={r.id} className="flex items-center justify-between text-sm">
                      <span className="font-medium"><span className="text-muted-foreground mr-2">{i + 1}.</span>{r.title}</span>
                      <Badge variant="outline">{r.topic}</Badge>
                    </li>
                  ))}
                </ul>
              )}
            </CardContent>
          </Card>
          <Card>
            <CardHeader><CardTitle className="text-base">Membership Distribution</CardTitle></CardHeader>
            <CardContent>
              {audience.length === 0 ? <p className="text-sm text-muted-foreground">No data yet.</p> : (
                <ul className="space-y-3">
                  {audience.map((a) => (
                    <li key={a.membershipTier} className="flex items-center justify-between text-sm">
                      <span className="font-medium">{a.membershipTier}</span>
                      <span className="text-muted-foreground">{a.userCount} users</span>
                    </li>
                  ))}
                </ul>
              )}
            </CardContent>
          </Card>
        </div>
      </div>
    </SiteShell>
  );
}