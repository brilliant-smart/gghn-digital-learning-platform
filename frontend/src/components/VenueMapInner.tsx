import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import L from "leaflet";
import "leaflet/dist/leaflet.css";

import markerIcon2x from "leaflet/dist/images/marker-icon-2x.png";
import markerIcon from "leaflet/dist/images/marker-icon.png";
import markerShadow from "leaflet/dist/images/marker-shadow.png";

delete (L.Icon.Default.prototype as any)._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: markerIcon2x,
  iconUrl: markerIcon,
  shadowUrl: markerShadow,
});

const VENUE: [number, number] = [9.0579, 7.4951];

export function VenueMapInner() {
  return (
    <MapContainer center={VENUE} zoom={15} scrollWheelZoom={false} className="h-64 w-full rounded-xl">
      <TileLayer
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />
      <Marker position={VENUE}>
        <Popup>
          <strong>Transcorp Hilton Abuja</strong>
          <br />
          1 Aguiyi Ironsi Street, Maitama, Abuja
        </Popup>
      </Marker>
    </MapContainer>
  );
}