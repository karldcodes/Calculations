# Calculations

This app features

- Graffana for viewing logs, metrics and tracing
- Simple extensibilty for adding new calculations

## Running

### prerequits

- Docker desktop
- Visual Studio 2026
- node

Open slnx file in visual studio and press F5 or click Debug -> Start Debugging to start the application

## Obeserveability

I setup the code to use OTEL endpoints so the telemetry collector is decoupled from the implementation. Any tool that support OTEL endpoints could be swapped in. To keep it simple I have used grafanas lgtm image that groups various observability tools together to capture logs, metrics and traces.

The https://github.com/grafana/docker-otel-lgtm image provides

- OpenTelemetry Collector
- Prometheus (metrics)
- Tempo (traces)
- Loki (logs)
- Pyroscope (profiles) 
- Grafana (Dashboards)