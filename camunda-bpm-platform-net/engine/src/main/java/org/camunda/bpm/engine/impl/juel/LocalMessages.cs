using System.Text;

/*
 * Based on JUEL 2.2.1 code, 2006-2009 Odysseus Software GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.impl.juel
{

	public sealed class LocalMessages
	{
		private const string BUNDLE_NAME = "org.camunda.bpm.engine.impl.juel.misc.LocalStrings";
		private static readonly ResourceBundle RESOURCE_BUNDLE = ResourceBundle.getBundle(BUNDLE_NAME);

		public static string get(string key, params object[] args)
		{
			string template = null;
			try
			{
				template = RESOURCE_BUNDLE.getString(key);
			}
			catch (MissingResourceException)
			{
				StringBuilder b = new StringBuilder();
				try
				{
					b.Append(RESOURCE_BUNDLE.getString("message.unknown"));
					b.Append(": ");
				}
				catch (MissingResourceException)
				{
				}
				b.Append(key);
				if (args != null && args.Length > 0)
				{
					b.Append("(");
					b.Append(args[0]);
					for (int i = 1; i < args.Length; i++)
					{
						b.Append(", ");
						b.Append(args[i]);
					}
					b.Append(")");
				}
				return b.ToString();
			}
			return MessageFormat.format(template, args);
		}
	}

}