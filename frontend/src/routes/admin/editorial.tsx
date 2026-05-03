import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { SiteShell } from "@/components/layout/SiteShell";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Textarea } from "@/components/ui/textarea";
import { Shield, CheckCircle2, XCircle, FileText } from "lucide-react";
import { editorialApi, type ResourceInReviewDto } from "@/api/editorial";
import { useAuth } from "@/lib/auth";
import { useNavigate } from "@tanstack/react-router";

export const Route = createFileRoute("/admin/editorial")({
  head: () => ({ meta: [{ title: "Editorial Queue | GGHN Admin" }] }),
  component: EditorialQueuePage,
});

function EditorialQueuePage() {
  const { user, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const [queue, setQueue] = useState<ResourceInReviewDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [rejectingId, setRejectingId] = useState<string | null>(null);
  const [rejectReason, setRejectReason] = useState("");

  useEffect(() => {
    if (!isAuthenticated || !user?.roles?.some(r => r === "Admin" || r === "Editor")) {
      navigate({ to: "/dashboard", replace: true });
      return;
    }
    editorialApi.getQueue()
      .then(setQueue)
      .catch(() => {})
      .finally(() => setLoading(false));
  }, [isAuthenticated]);

  const handleApprove = async (resourceId: string) => {
    try {
      await editorialApi.approve(resourceId);
      setQueue(q => q.filter(r => r.id !== resourceId));
    } catch (err) { console.error(err); }
  };

  const handleReject = async (resourceId: string) => {
    try {
      await editorialApi.reject(resourceId, rejectReason);
      setQueue(q => q.filter(r => r.id !== resourceId));
      setRejectingId(null);
      setRejectReason("");
    } catch (err) { console.error(err); }
  };

  if (loading) return <SiteShell><div className="flex justify-center py-20"><p className="text-muted-foreground">Loading...</p></div></SiteShell>;

  const statusColor = (s: string) => s === "UnderReview" ? "bg-yellow-100 text-yellow-800" : "bg-gray-100 text-gray-800";

  return (
    <SiteShell>
      <div className="mx-auto max-w-5xl px-4 py-10 sm:px-6 lg:px-8">
        <div className="flex items-center gap-3 mb-8">
          <Shield className="h-6 w-6 text-primary" />
          <h1 className="text-2xl font-bold tracking-tight">Editorial Queue</h1>
        </div>
        {queue.length === 0 ? (
          <Card><CardContent className="py-10 text-center text-muted-foreground">No resources pending review.</CardContent></Card>
        ) : (
          <div className="space-y-4">
            {queue.map((r) => (
              <Card key={r.id}>
                <CardContent className="py-5">
                  <div className="flex items-start justify-between gap-4">
                    <div className="flex-1 min-w-0">
                      <h3 className="font-semibold text-foreground">{r.title}</h3>
                      <p className="mt-1 text-sm text-muted-foreground line-clamp-2">{r.summary}</p>
                      <div className="mt-2 flex items-center gap-2">
                        <Badge className={statusColor(r.status)}>{r.status}</Badge>
                        <span className="text-xs text-muted-foreground">{new Date(r.createdAt).toLocaleDateString()}</span>
                      </div>
                    </div>
                    <div className="flex flex-col gap-2">
                      {rejectingId === r.id ? (
                        <div className="space-y-2">
                          <Textarea placeholder="Reason for rejection..." value={rejectReason} onChange={(e) => setRejectReason(e.target.value)} rows={2} />
                          <div className="flex gap-2">
                            <Button size="sm" variant="destructive" onClick={() => handleReject(r.id)}>Confirm Reject</Button>
                            <Button size="sm" variant="outline" onClick={() => { setRejectingId(null); setRejectReason(""); }}>Cancel</Button>
                          </div>
                        </div>
                      ) : (
                        <>
                          <Button size="sm" onClick={() => handleApprove(r.id)}><CheckCircle2 className="mr-1.5 h-3.5 w-3.5" />Approve</Button>
                          <Button size="sm" variant="outline" className="text-destructive" onClick={() => setRejectingId(r.id)}><XCircle className="mr-1.5 h-3.5 w-3.5" />Reject</Button>
                        </>
                      )}
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        )}
      </div>
    </SiteShell>
  );
}