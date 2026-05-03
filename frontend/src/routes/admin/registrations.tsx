import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { SiteShell } from "@/components/layout/SiteShell";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Textarea } from "@/components/ui/textarea";
import { Users, CheckCircle2, XCircle, Clock, AlertCircle } from "lucide-react";
import { conferenceApi, type RegistrationDto, type RegistrationStatsDto, type ConferenceDto } from "@/api/conferences";
import { useAuth } from "@/lib/auth";
import { useNavigate } from "@tanstack/react-router";

export const Route = createFileRoute("/admin/registrations")({
  head: () => ({ meta: [{ title: "Registration Approvals | GGHN Admin" }] }),
  component: AdminRegistrationsPage,
});

const STATUS_COLORS: Record<string, string> = {
  Pending: "bg-yellow-100 text-yellow-700",
  Approved: "bg-green-100 text-green-700",
  Rejected: "bg-red-100 text-red-700",
  Waitlisted: "bg-blue-100 text-blue-700",
  Cancelled: "bg-muted text-muted-foreground",
};

function AdminRegistrationsPage() {
  const { user, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const [conferences, setConferences] = useState<ConferenceDto[]>([]);
  const [selectedConf, setSelectedConf] = useState<string>("");
  const [registrations, setRegistrations] = useState<RegistrationDto[]>([]);
  const [stats, setStats] = useState<RegistrationStatsDto | null>(null);
  const [filter, setFilter] = useState<string>("");
  const [loading, setLoading] = useState(true);
  const [rejectId, setRejectId] = useState<string | null>(null);
  const [rejectReason, setRejectReason] = useState("");

  useEffect(() => {
    if (!isAuthenticated || !user?.roles?.some((r: string) => r === "Admin" || r === "Editor")) {
      navigate({ to: "/dashboard", replace: true });
      return;
    }
    conferenceApi.getAll().catch(() => [] as ConferenceDto[]).then((c) => {
      setConferences(c);
      if (c.length > 0) {
        setSelectedConf(c[0].id);
      }
      setLoading(false);
    });
  }, [isAuthenticated]);

  useEffect(() => {
    if (!selectedConf) return;
    Promise.all([
      conferenceApi.getRegistrationsByConference(selectedConf, filter || undefined).catch(() => [] as RegistrationDto[]),
      conferenceApi.getRegistrationStats(selectedConf).catch(() => null as RegistrationStatsDto | null),
    ]).then(([r, s]) => {
      setRegistrations(r);
      setStats(s);
    });
  }, [selectedConf, filter]);

  const handleApprove = async (id: string) => {
    await conferenceApi.updateRegistrationStatus(id, { status: "Approved" });
    setRegistrations((r) => r.map((x) => (x.id === id ? { ...x, status: "Approved" } : x)));
    if (stats) setStats({ ...stats, pending: stats.pending - 1, approved: stats.approved + 1 });
  };

  const handleReject = async () => {
    if (!rejectId) return;
    await conferenceApi.updateRegistrationStatus(rejectId, { status: "Rejected", rejectionReason: rejectReason });
    setRegistrations((r) => r.map((x) => (x.id === rejectId ? { ...x, status: "Rejected", rejectionReason: rejectReason } : x)));
    if (stats) setStats({ ...stats, pending: stats.pending - 1, rejected: stats.rejected + 1 });
    setRejectId(null);
    setRejectReason("");
  };

  if (loading) return <SiteShell><div className="flex justify-center py-20"><p className="text-muted-foreground">Loading...</p></div></SiteShell>;

  return (
    <SiteShell>
      <div className="mx-auto max-w-6xl px-4 py-10 sm:px-6 lg:px-8">
        <h1 className="text-2xl font-bold tracking-tight mb-2">Registration Approvals</h1>
        <p className="text-muted-foreground text-sm mb-8">Review and manage conference registrations.</p>

        {/* Conference selector */}
        <div className="mb-6">
          <select
            value={selectedConf}
            onChange={(e) => setSelectedConf(e.target.value)}
            className="rounded-md border px-3 py-2 text-sm bg-background"
          >
            {conferences.map((c) => (
              <option key={c.id} value={c.id}>{c.title}</option>
            ))}
          </select>
        </div>

        {/* Stats */}
        {stats && (
          <div className="grid grid-cols-5 gap-4 mb-6">
            {[
              { icon: Users, label: "Total", value: stats.totalRegistrations, color: "text-foreground" },
              { icon: Clock, label: "Pending", value: stats.pending, color: "text-yellow-600" },
              { icon: CheckCircle2, label: "Approved", value: stats.approved, color: "text-green-600" },
              { icon: XCircle, label: "Rejected", value: stats.rejected, color: "text-red-600" },
              { icon: AlertCircle, label: "Waitlisted", value: stats.waitlisted, color: "text-blue-600" },
            ].map((s) => (
              <Card key={s.label}>
                <CardContent className="flex items-center gap-3 p-4">
                  <s.icon className={`h-5 w-5 ${s.color}`} />
                  <div>
                    <p className="text-xl font-bold">{s.value}</p>
                    <p className="text-[10px] text-muted-foreground uppercase tracking-wider">{s.label}</p>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        )}

        {/* Filter tabs */}
        <div className="flex gap-2 mb-4">
          {["", "Pending", "Approved", "Rejected", "Waitlisted"].map((f) => (
            <Button key={f} variant={filter === f ? "default" : "outline"} size="sm" onClick={() => setFilter(f)}>
              {f || "All"}
            </Button>
          ))}
        </div>

        {/* Registrations table */}
        <Card>
          <CardContent className="p-0">
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead className="border-b bg-muted/50">
                  <tr>
                    <th className="px-4 py-3 text-left font-medium">Name</th>
                    <th className="px-4 py-3 text-left font-medium">Organization</th>
                    <th className="px-4 py-3 text-left font-medium">Country</th>
                    <th className="px-4 py-3 text-left font-medium">Type</th>
                    <th className="px-4 py-3 text-left font-medium">Date</th>
                    <th className="px-4 py-3 text-left font-medium">Status</th>
                    <th className="px-4 py-3 text-left font-medium">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {registrations.length === 0 ? (
                    <tr><td colSpan={7} className="px-4 py-10 text-center text-muted-foreground">No registrations found.</td></tr>
                  ) : (
                    registrations.map((r) => (
                      <tr key={r.id} className="border-b last:border-0 hover:bg-muted/30">
                        <td className="px-4 py-3">
                          <p className="font-medium">{r.firstName} {r.lastName}</p>
                          <p className="text-xs text-muted-foreground">{r.email}</p>
                        </td>
                        <td className="px-4 py-3 text-muted-foreground">
                          <p>{r.organization}</p>
                          <p className="text-xs">{r.jobTitle}</p>
                        </td>
                        <td className="px-4 py-3">{r.country}</td>
                        <td className="px-4 py-3"><Badge variant="outline" className="text-xs">{r.registrationType}</Badge></td>
                        <td className="px-4 py-3 text-xs text-muted-foreground">{new Date(r.createdAt).toLocaleDateString()}</td>
                        <td className="px-4 py-3">
                          <Badge className={`text-xs ${STATUS_COLORS[r.status] || ""}`}>{r.status}</Badge>
                          {r.rejectionReason && <p className="text-xs text-muted-foreground mt-1 max-w-[150px] truncate">{r.rejectionReason}</p>}
                        </td>
                        <td className="px-4 py-3">
                          {r.status === "Pending" && (
                            <div className="flex gap-1">
                              <Button size="sm" variant="outline" className="h-7 text-xs text-green-600" onClick={() => handleApprove(r.id)}>
                                Approve
                              </Button>
                              <Button size="sm" variant="outline" className="h-7 text-xs text-destructive" onClick={() => { setRejectId(r.id); setRejectReason(""); }}>
                                Reject
                              </Button>
                            </div>
                          )}
                          {r.status !== "Pending" && (
                            <span className="text-xs text-muted-foreground">
                              {r.reviewedAt ? new Date(r.reviewedAt).toLocaleDateString() : "—"}
                            </span>
                          )}
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </CardContent>
        </Card>

        {/* Reject modal */}
        {rejectId && (
          <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm">
            <Card className="w-full max-w-md">
              <CardHeader><CardTitle>Reject Registration</CardTitle></CardHeader>
              <CardContent>
                <Label>Reason for rejection</Label>
                <Textarea
                  value={rejectReason}
                  onChange={(e) => setRejectReason(e.target.value)}
                  placeholder="Provide a reason for rejection..."
                  rows={3}
                  className="mt-2"
                />
                <div className="mt-4 flex justify-end gap-2">
                  <Button variant="outline" size="sm" onClick={() => setRejectId(null)}>Cancel</Button>
                  <Button size="sm" variant="destructive" onClick={handleReject}>Confirm Rejection</Button>
                </div>
              </CardContent>
            </Card>
          </div>
        )}
      </div>
    </SiteShell>
  );
}
