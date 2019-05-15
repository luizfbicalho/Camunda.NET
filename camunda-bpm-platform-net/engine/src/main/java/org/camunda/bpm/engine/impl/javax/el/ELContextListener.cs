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
namespace org.camunda.bpm.engine.impl.javax.el
{
	/// <summary>
	/// The listener interface for receiving notification when an <seealso cref="ELContext"/> is created.
	/// </summary>
	public interface ELContextListener : java.util.EventListener
	{
		/// <summary>
		/// Invoked when a new ELContext has been created.
		/// </summary>
		/// <param name="ece">
		///            the notification event. </param>
		void contextCreated(ELContextEvent ece);
	}

}