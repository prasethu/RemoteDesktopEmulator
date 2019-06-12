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

            int version;
            if (!TryParseInt32(queue, out version) || (version != Version))
                return fallbackModel;

            Model model;
            if (!TryParseModel(queue, out model))
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

            int displaysCount;
            if (!TryParseInt32(queue, out displaysCount))
                return false;

            HashSet<Display> displays = new HashSet<Display>();

            Display display;
            for (int ii = 0; ii < displaysCount; ii++)
            {
                if (TryParseDisplay(queue, out display))
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

            bool isPrimary;
            if (!TryParseBoolean(queue, out isPrimary))
                return false;

            Resolution resolution;
            if (!TryParseResolution(queue, out resolution))
                return false;

            Scale scale;
            if (!TryParseScale(queue, out scale))
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

            int selectedIndex;
            if (!TryParseInt32(queue, out selectedIndex))
                return false;

            int configurationsCount;
            if (!TryParseInt32(queue, out configurationsCount))
                return false;

            HashSet<Configuration> configurations = new HashSet<Configuration>();

            Configuration configuration;
            for (int ii = 0; ii < configurationsCount; ii++)
            {
                if (TryParseConfiguration(queue, out configuration))
                    configurations.Add(configuration);
            }

            if (configurations.Count != configurationsCount)
                return false;

            Settings settings;
            if (!TryParseSettings(queue, out settings))
                return false;

            model = Model.Create(configurations.ToList(), selectedIndex, settings);

            return true;
        }

        private static bool TryParseResolution(Queue<string> queue, out Resolution resolution)
        {
            resolution = Resolution.CreateDefault();

            int height;
            if (!TryParseInt32(queue, out height))
                return false;

            int width;
            if (!TryParseInt32(queue, out width))
                return false;

            resolution = Resolution.Create(width, height);

            return true;
        }

        private static bool TryParseScale(Queue<string> queue, out Scale scale)
        {
            scale = Scale.CreateDefault();

            int value;
            if (!TryParseInt32(queue, out value))
                return false;

            scale = Scale.Create(value);

            return true;
        }

        private static bool TryParseSettings(Queue<string> queue, out Settings settings)
        {
            settings = Settings.CreateDefault();

            int resolutionsCount;
            if (!TryParseInt32(queue, out resolutionsCount))
                return false;

            HashSet<Resolution> resolutions = new HashSet<Resolution>();

            Resolution resolution;
            for (int ii = 0; ii < resolutionsCount; ii++)
            {
                if (TryParseResolution(queue, out resolution))
                    resolutions.Add(resolution);
            }

            if (resolutions.Count != resolutionsCount)
                return false;

            int scalesCount;
            if (!TryParseInt32(queue, out scalesCount))
                return false;

            HashSet<Scale> scales = new HashSet<Scale>();

            Scale scale;
            for (int ii = 0; ii < scalesCount; ii++)
            {
                if (TryParseScale(queue, out scale))
                    scales.Add(scale);
            }

            if (scales.Count != scalesCount)
                return false;

            settings = Settings.Create(resolutions.ToList(), scales.ToList());

            return true;
        }
    }
}
