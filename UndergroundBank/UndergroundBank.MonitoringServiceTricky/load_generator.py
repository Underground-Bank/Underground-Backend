import logging
import os
import random
import time
from datetime import datetime
import grpc

from opentelemetry import trace, metrics
from opentelemetry.trace import Status, StatusCode
from opentelemetry.sdk.resources import Resource

from opentelemetry.sdk.trace import TracerProvider
from opentelemetry.sdk.trace.export import BatchSpanProcessor
from opentelemetry.exporter.otlp.proto.grpc.trace_exporter import OTLPSpanExporter

from opentelemetry._logs import set_logger_provider
from opentelemetry.sdk._logs import LoggerProvider, LoggingHandler
from opentelemetry.sdk._logs.export import BatchLogRecordProcessor
from opentelemetry.exporter.otlp.proto.grpc._log_exporter import OTLPLogExporter

from opentelemetry.sdk.metrics import MeterProvider
from opentelemetry.sdk.metrics.export import PeriodicExportingMetricReader
from opentelemetry.exporter.otlp.proto.grpc.metric_exporter import OTLPMetricExporter

OTLP_ENDPOINT = "localhost:4320"

resource = Resource.create({
    "service.name": "authService",
    "service.version": "1.0.0",
    "service.instance.id": f"demo-{os.getpid()}"
})

trace_provider = TracerProvider(resource=resource)
trace.set_tracer_provider(trace_provider)
trace_exporter = OTLPSpanExporter(endpoint=OTLP_ENDPOINT, insecure=True)
trace_provider.add_span_processor(BatchSpanProcessor(trace_exporter))
tracer = trace.get_tracer(__name__)

log_provider = LoggerProvider(resource=resource)
set_logger_provider(log_provider)
log_exporter = OTLPLogExporter(endpoint=OTLP_ENDPOINT, insecure=True)
log_provider.add_log_record_processor(BatchLogRecordProcessor(log_exporter))
handler = LoggingHandler(level=logging.INFO, logger_provider=log_provider)
logger = logging.getLogger("demo")
logger.setLevel(logging.INFO)
logger.addHandler(handler)

metric_exporter = OTLPMetricExporter(endpoint=OTLP_ENDPOINT, insecure=True)
reader = PeriodicExportingMetricReader(metric_exporter, export_interval_millis=5000)
meter_provider = MeterProvider(resource=resource, metric_readers=[reader])
metrics.set_meter_provider(meter_provider)
meter = metrics.get_meter(__name__)
auth_counter = meter.create_counter("auth_requests_total", unit="{requests}", description="Всего auth-запросов")
auth_duration = meter.create_histogram("auth_duration_ms", unit="ms", description="Длительность auth-запросов")

def simulate_auth() -> tuple[int, float]:
    status = random.choices([200, 401, 500], weights=[0.8, 0.15, 0.05])[0]
    duration = random.uniform(50, 300) if status == 200 else random.uniform(10, 100)
    time.sleep(duration / 1000)
    return status, duration

def test_grpc_connection():
    try:
        channel = grpc.insecure_channel("localhost:4320")
        grpc.channel_ready_future(channel).result(timeout=5)
        print("✅ gRPC-соединение с localhost:4320 успешно!")
    except Exception as e:
        print(f"❌ Ошибка соединения: {e}")


if __name__ == "__main__":
    test_grpc_connection()
    print(f"[{datetime.now():%H:%M:%S}] Sending OTLP data via gRPC to {OTLP_ENDPOINT}")
    iteration = 0

    while True:
        iteration += 1
        with tracer.start_as_current_span("auth") as span:
            span.set_attribute("iteration", iteration)

            status, dur = simulate_auth()
            auth_counter.add(1, {"status_code": status})
            auth_duration.record(dur, {"status_code": status})

            msg = f"[auth] status={status} duration={dur:.1f}ms"
            if status == 200:
                logger.info(msg)
            else:
                logger.warning(msg)
                span.record_exception(RuntimeError(msg))
                span.set_status(Status(StatusCode.ERROR, msg))
