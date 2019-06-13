using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteDesktopEmulator
{
    internal class Serializer
    {
        private const int Version = 1;

        public const char Separator = ';';

        public static Model Deserialize(string data)
        {
            Model fallbackModel = Model.CreateDefault();

            if (string.IsNullOrWhiteSpace(data))
                return fallbackModel;

            Queue<string> queue = new Queue<string>(data.Split(Separator));

            if (!TryParseInt32(queue, out int version) || (version != Version))
                return fallbackModel;

            if (!TryParseModel(queue, out Model model))
                return fallbackModel;

            return model;
        }

        public static string Serialize(Model model)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(Version);
            builder.Append(Separator);

            SerializeModel(builder, model);

            return builder.ToString();
        }

        private static void SerializeConfiguration(StringBuilder builder, Configuration configuration)
        {
            if (configuration == null)
                return;

            builder.Append(configuration.Displays.Count);
            builder.Append(Separator);

            foreach (Display display in configuration.Displays)
            {
                SerializeDisplay(builder, display);
                builder.Append(Separator);
            }

            builder.Append(configuration.Name);
        }

        private static void SerializeDisplay(StringBuilder builder, Display display)
        {
            if (display == null)
                return;

            builder.Append(display.IsPrimary);
            builder.Append(Separator);

            SerializeResolution(builder, display.Resolution);

            builder.Append(Separator);

            SerializeScale(builder, display.Scale);
        }

        private static void SerializeModel(StringBuilder builder, Model model)
        {
            if (model == null)
                return;

            builder.Append(model.SelectedIndex);
            builder.Append(Separator);
            builder.Append(model.Configurations.Count);
            builder.Append(Separator);

            foreach (Configuration configuration in model.Configurations)
            {
                SerializeConfiguration(builder, configuration);
                builder.Append(Separator);
            }

            SerializeSettings(builder, model.Settings);
        }

        private static void SerializeResolution(StringBuilder builder, Resolution resolution)
        {
            if (resolution == null)
                return;

            builder.Append(resolution.Height);
            builder.Append(Separator);
            builder.Append(resolution.Width);
        }

        private static void SerializeScale(StringBuilder builder, Scale scale)
        {
            if (scale == null)
                return;

            builder.Append(scale.Value);
        }

        private static void SerializeSettings(StringBuilder builder, Settings settings)
        {
            if (settings == null)
                return;

            builder.Append(settings.AvailableResolutions.Count);
            builder.Append(Separator);

            foreach (Resolution resolution in settings.AvailableResolutions)
            {
                SerializeResolution(builder, resolution);
                builder.Append(Separator);
            }

            builder.Append(settings.AvailableScales.Count);
            builder.Append(Separator);

            foreach (Scale scale in settings.AvailableScales)
            {
                SerializeScale(builder, scale);
                builder.Append(Separator);
            }
        }

        private static bool TryParseBoolean(Queue<string> queue, out bool value)
        {
            return bool.TryParse(queue.Dequeue(), out value);
        }

        private static bool TryParseConfiguration(Queue<string> queue, out Configuration configuration)
        {
            configuration = Configuration.CreateDefault();

            if (!TryParseInt32(queue, out int displaysCount))
                return false;

            HashSet<Display> displays = new HashSet<Display>();

            for (int ii = 0; ii < displaysCount; ii++)
            {
                if (TryParseDisplay(queue, out Display display))
                    displays.Add(display);
            }

            if (displays.Count != displaysCount)
                return false;

            string name = queue.Dequeue();

            configuration = Configuration.Create(name, displays.ToList());

            return true;
        }

        private static bool TryParseDisplay(Queue<string> queue, out Display display)
        {
            display = Display.CreateDefault();

            if (!TryParseBoolean(queue, out bool isPrimary))
                return false;

            if (!TryParseResolution(queue, out Resolution resolution))
                return false;

            if (!TryParseScale(queue, out Scale scale))
                return false;

            display = Display.Create(isPrimary, resolution, scale);

            return true;
        }

        private static bool TryParseInt32(Queue<string> queue, out int value)
        {
            return int.TryParse(queue.Dequeue(), out value);
        }

        private static bool TryParseModel(Queue<string> queue, out Model model)
        {
            model = Model.CreateDefault();

            if (!TryParseInt32(queue, out int selectedIndex))
                return false;

            if (!TryParseInt32(queue, out int configurationsCount))
                return false;

            HashSet<Configuration> configurations = new HashSet<Configuration>();

            for (int ii = 0; ii < configurationsCount; ii++)
            {
                if (TryParseConfiguration(queue, out Configuration configuration))
                    configurations.Add(configuration);
            }

            if (configurations.Count != configurationsCount)
                return false;

            if (!TryParseSettings(queue, out Settings settings))
                return false;

            model = Model.Create(configurations.ToList(), selectedIndex, settings);

            return true;
        }

        private static bool TryParseResolution(Queue<string> queue, out Resolution resolution)
        {
            resolution = Resolution.CreateDefault();

            if (!TryParseInt32(queue, out int height))
                return false;

            if (!TryParseInt32(queue, out int width))
                return false;

            resolution = Resolution.Create(width, height);

            return true;
        }

        private static bool TryParseScale(Queue<string> queue, out Scale scale)
        {
            scale = Scale.CreateDefault();

            if (!TryParseInt32(queue, out int value))
                return false;

            scale = Scale.Create(value);

            return true;
        }

        private static bool TryParseSettings(Queue<string> queue, out Settings settings)
        {
            settings = Settings.CreateDefault();

            if (!TryParseInt32(queue, out int resolutionsCount))
                return false;

            HashSet<Resolution> resolutions = new HashSet<Resolution>();

            for (int ii = 0; ii < resolutionsCount; ii++)
            {
                if (TryParseResolution(queue, out Resolution resolution))
                    resolutions.Add(resolution);
            }

            if (resolutions.Count != resolutionsCount)
                return false;

            if (!TryParseInt32(queue, out int scalesCount))
                return false;

            HashSet<Scale> scales = new HashSet<Scale>();

            for (int ii = 0; ii < scalesCount; ii++)
            {
                if (TryParseScale(queue, out Scale scale))
                    scales.Add(scale);
            }

            if (scales.Count != scalesCount)
                return false;

            settings = Settings.Create(resolutions.ToList(), scales.ToList());

            return true;
        }
    }
}
